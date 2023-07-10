using MateMachine.Data.Models;
using System.Threading.Tasks;

namespace MateMachine.Admin.Services.Contracts
{
    public interface IAccountService
    {
        public Task<string> RegisterByEmailAsync(string userName, string email, string phoneNumber, string fullName, string password);
        public Task<string> RegisterByPhoneNumberAsync(string userName, string email, string phoneNumber, string fullName, string password);
        public Task ConfirmEmailAsync(string userName, string token);
        public Task ConfirmPhoneNumberAsync(string userName, string token);
        public Task<string> AuthenticateAsync(string userName, string password);
        public Task<User> GetCurrentUserAsync();
        public Task ChangePasswordAsync(string currentPassword, string newPassword);
        public Task<string> GenerateChangeEmailTokenAsync(string newEmail);
        public Task<string> GenerateChangePhoneNumberTokenAsync(string newPhoneNumber);
        public Task ChangeEmailAsync(string newEmail, string token);
        public Task ChangePhoneNumberAsync(string newPhoneNumber, string token);
        public Task<string> GeneratePasswordResetTokenAsync(string userName);
        public Task ResetPasswordAsync(string userName, string token, string newPassword);
    }
}
