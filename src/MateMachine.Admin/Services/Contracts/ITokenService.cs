using MateMachine.Data.Models;
using System.Collections.Generic;

namespace MateMachine.Admin.Services.Contracts
{
    public interface ITokenService
    {
        public string GenerateToken(User user, IList<string> roles);
    }
}
