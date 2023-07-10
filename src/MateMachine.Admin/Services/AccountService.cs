

using MateMachine.Admin.Services.Contracts;
using MateMachine.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace MateMachine.Admin.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        readonly IClaimService _claimService;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<User> userManager, IClaimService claimService, ITokenService tokenService)
        {
            _userManager = userManager;
            _claimService = claimService;
            _tokenService = tokenService;
        }

        public async Task<string> RegisterByEmailAsync(string userName, string email, string phoneNumber, string fullName, string password)
        {
            var user = await CreateNewUserAsync(userName, email, phoneNumber, fullName, password);
            await AddToUserRole(user);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public async Task<string> RegisterByPhoneNumberAsync(string userName, string email, string phoneNumber, string fullName, string password)
        {
            var user = await CreateNewUserAsync(userName, email, phoneNumber, fullName, password);
            await AddToUserRole(user);
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            return token;
        }

        private async Task<User> CreateNewUserAsync(string userName, string email, string phoneNumber, string firstName, string password)
        {
            var user = new User
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                SilverBio = string.Empty,
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result != IdentityResult.Success)
            {
                throw new TaskCanceledException(result.ToString());
            }
            return user;
        }

        private async Task AddToUserRole(User user)
        {
            var result = await _userManager.AddToRoleAsync(user, "User");
            if (result != IdentityResult.Success)
            {
                throw new TaskCanceledException(result.ToString());
            }
        }

        private async Task<User> GetUserByName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new KeyNotFoundException($"Couldn't find any user with UserName = {userName}");
            return user;
        }

        public async Task ConfirmEmailAsync(string userName, string token)
        {
            var user = await GetUserByName(userName);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result != IdentityResult.Success)
                throw new Exception(result.ToString());
        }

        public async Task ConfirmPhoneNumberAsync(string userName, string token)
        {
            var user = await GetUserByName(userName);
            //var isTokenValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, user.PhoneNumber, token);
            var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, token);
            if (result != IdentityResult.Success)
                throw new Exception(result.ToString());
        }

        public async Task<string> AuthenticateAsync(string userName, string password)
        {
            var user = await GetUserByName(userName);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new InvalidCredentialException("Incorrect  Username or Password");
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            var isPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            if (!(isEmailConfirmed || isPhoneNumberConfirmed))
                throw new InvalidCredentialException("Please Confirm Email or PhoneNumber");

            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenService.GenerateToken(user, roles);
            return jwtToken;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var userName = _claimService.GetUserName();
            var user = await GetUserByName(userName);
            return user;
        }

        public async Task ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var user = await GetCurrentUserAsync();
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result != IdentityResult.Success)
                throw new Exception(result.ToString());
        }

        public async Task<string> GenerateChangeEmailTokenAsync(string newEmail)
        {
            var user = await GetCurrentUserAsync();
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            return token;
        }

        public async Task<string> GenerateChangePhoneNumberTokenAsync(string newPhoneNumber)
        {
            var user = await GetCurrentUserAsync();
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);
            return token;
        }

        public async Task ChangeEmailAsync(string newEmail, string token)
        {
            var user = await GetCurrentUserAsync();
            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (result != IdentityResult.Success)
                throw new Exception(result.ToString());
        }

        public async Task ChangePhoneNumberAsync(string newPhoneNumber, string token)
        {
            var user = await GetCurrentUserAsync();
            var result = await _userManager.ChangePhoneNumberAsync(user, newPhoneNumber, token);
            if (result != IdentityResult.Success)
                throw new Exception(result.ToString());
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string userName)
        {
            var user = await GetCurrentUserAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task ResetPasswordAsync(string userName, string token, string newPassword)
        {
            var user = await GetUserByName(userName);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result != IdentityResult.Success)
                throw new Exception(result.ToString());
        }

    }
}
