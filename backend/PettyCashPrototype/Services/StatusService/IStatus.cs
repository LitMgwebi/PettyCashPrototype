﻿namespace PettyCashPrototype.Services.StatusService
{
    public interface IStatus
    {
        public Task<IEnumerable<Status>> GetApproved();
        public Task<IEnumerable<Status>> GetRecommended();
    }
}