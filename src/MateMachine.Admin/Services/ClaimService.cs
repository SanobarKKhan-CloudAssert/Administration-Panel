
using MateMachine.Admin.Services.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace MateMachine.Admin.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim?.Value))
            {
                return null;
            }
            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return null;
            }
            return userId;
        }

        public string GetUserName()
        {
            var userNameClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (string.IsNullOrEmpty(userNameClaim?.Value))
            {
                return null;
            }
            return userNameClaim.Value;
        }
    }
}
