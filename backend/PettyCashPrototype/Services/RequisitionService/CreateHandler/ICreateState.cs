﻿namespace PettyCashPrototype.Services.RequisitionService.CreateHandler
{
    public interface ICreateState
    {
        public Task<string> CreateRequisition(Requisition requisition, PettyCashPrototypeContext _db, string userId);
    }
}
