﻿using PettyCashPrototype.Services.RequisitionService.CreateHandler;
using PettyCashPrototype.Services.RequisitionService.EditHandler;
using PettyCashPrototype.Services.RequisitionService.IndexHandler;

namespace PettyCashPrototype.Services.RequisitionService
{
    public class RequisitionService : IRequisition
    {
        private PettyCashPrototypeContext _db;
        private readonly IUser _user;
        private readonly IGLAccount _glAccount;
        private readonly IJobTitle _jobTitle;
        public RequisitionService(PettyCashPrototypeContext db, IUser user, IGLAccount gLAccount, IJobTitle jobTitle)
        {
            _db = db;
            _user = user;
            _glAccount = gLAccount;
            _jobTitle = jobTitle;
        }

        public async Task<IEnumerable<Requisition>> GetAll(string command, int divisionId, int jobTitleId, string userId, string role)
        {
            try
            {
                GetRequisitionsHandler indexHandler = new GetRequisitionsHandler();
                IEnumerable<Requisition> requisitions = new List<Requisition>();

                if (command == "all")
                {
                    indexHandler.setState(new GetAllState(_db));
                    requisitions = await indexHandler.request();
                }
                else if (command == "forOne")
                {
                    indexHandler.setState(new GetForApplicantState(_db, userId));
                    requisitions = await indexHandler.request();
                }
                else if (command == "recommendation")
                {
                    indexHandler.setState(new GetForRecommendationState(_user, _db, userId, role));
                    requisitions = await indexHandler.request();
                }
                else if (command == "approval")
                {
                    indexHandler.setState(new GetForApprovalState(_db, divisionId, jobTitleId, _jobTitle, userId));
                    requisitions = await indexHandler.request();
                }
                else if (command == "issuing")
                {
                    indexHandler.setState(new GetForIssuingState(_user, _db, userId));
                    requisitions = await indexHandler.request();
                }
                else
                    throw new NotImplementedException("Could not resolve issue when retrieving requisitions");

                if (requisitions == null) throw new Exception("System could not find any of your requisition forms.");
                return requisitions;
            }
            catch { throw; }
        }

        public async Task<Requisition> GetOne(int id)
        {
            try
            {
                Requisition requisition = await _db.Requisitions
                    .Include(m => m.ManagerRecommendation)
                    .Include(f => f.FinanceApproval)
                    .Include(m => m.Manager)
                    .Include(f => f.FinanceOfficer)
                    .Include(z => z.Applicant)
                    .Include(gl => gl.Glaccount)
                    .Where(a => a.IsActive == true)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.RequisitionId == id);

                //requisition!.Motivations = await _motivation.GetAllByRequisition(id);

                if (requisition == null) throw new Exception("System could not retrieve the Requisition requested.");
                return requisition;
            }
            catch { throw; }
        }

        public async Task<string> Create(Requisition requisition, string userId)
        {
            try
            {
                Glaccount glaccount = await _glAccount.GetOne(requisition.GlaccountId);
                CreateRequisitionHandler createHandler = new CreateRequisitionHandler();
                string message = string.Empty;

                if (glaccount.NeedsMotivation == true || requisition.AmountRequested > 2000)
                {
                    createHandler.setState(new RequireMotivationState(requisition, _db, userId));
                    message = await createHandler.request();
                }
                else if (glaccount.NeedsMotivation == false)
                {
                    createHandler.setState(new StandardCreateState(requisition, _db, userId));
                    message = await createHandler.request();
                } else
                {
                    throw new Exception("System could not resolve error within requisition creation.");
                }

                return message;
            }
            catch { throw; }
        }

        public async Task<string> Edit(Requisition requisition, string command, string userId, int attemptCode)
        {
            try
            {
                string messageResponse = "";
                EditRequisitionHandler editRequisition = new EditRequisitionHandler();
                Requisition reviewRequisition = await GetOne(requisition.RequisitionId);

                if (command == "recommendation")
                {
                    editRequisition.setState(new RecommendationState(_db, reviewRequisition, requisition, userId));
                    messageResponse = await editRequisition.request();
                }
                else if (command == "approval")
                {
                    editRequisition.setState(new ApprovalState(_db, reviewRequisition, requisition, userId));
                    messageResponse = await editRequisition.request();
                }
                else if (command == "edit")
                {
                    editRequisition.setState(new WholeRequisitionState(_db, requisition));
                    messageResponse = await editRequisition.request();
                }
                else if (command == "issuing")
                {
                    editRequisition.setState(new IssuingState(_db, requisition, userId, attemptCode));
                    messageResponse = await editRequisition.request();
                }
                else if (command == "addMotivation")
                {
                    requisition.Stage = "Motivation has been uploaded. Requisition has been sent for recommendation.";
                    editRequisition.setState(new WholeRequisitionState(_db, requisition));
                    messageResponse = await editRequisition.request();
                }
                else
                    throw new Exception("System could not resolve error within requisition editing.");

                return messageResponse;
            }
            catch { throw; }
        }

        public void SoftDelete(Requisition requisition)
        {
            try
            {
                requisition.IsActive = false;
                _db.Requisitions.Update(requisition);
                int result = _db.SaveChanges();

                if (result == 0) throw new DbUpdateException($"System could not delete the requested requisition.");
            }
            catch { throw; }
        }
    }
}
