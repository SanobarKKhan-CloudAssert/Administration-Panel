using System;

namespace MateMachine.Admin.Services.Contracts
{
    public interface IClaimService
    {
        public Guid? GetUserId();
        public string GetUserName();
    }
}