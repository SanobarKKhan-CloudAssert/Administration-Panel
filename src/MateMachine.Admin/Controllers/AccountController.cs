using MateMachine.Admin.Services.Contracts;
using MateMachine.Dto.Account.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MateMachine.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService authService, ILogger<AccountController> logger)
        {
            _accountService = authService;
            _logger = logger;
        }

        /// POST: api/Account/RegisterByEmail
        /// Create new user and send confirmation email to user.
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterByEmail(RegisterByEmailRequest request)
        {
            var token = await _accountService.RegisterByEmailAsync(
                userName: request.UserName,
                email: request.Email,
                phoneNumber: request.PhoneNumber,
                fullName: request.FullName,
                password: request.Password);
            // TODO: must send confirmation email to user.
            return Ok(token);
        }

        /// POST: api/Account/RegisterByEmail
        /// Create new user and send confirmation sms to user.
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterByPhoneNumber(RegisterByPhoneNumberRequest request)
        {
            var token = await _accountService.RegisterByPhoneNumberAsync(
                userName: request.UserName,
                email: request.Email,
                phoneNumber: request.PhoneNumber,
                fullName: request.FullName,
                password: request.Password);
            // TODO: must send confirmation sms to user.
            return Ok(token);
        }

        /// POST: api/Account/ConfirmEmail
        /// Confirm email.
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            await _accountService.ConfirmEmailAsync(userName: request.UserName, token: request.Token);
            return Ok();
        }

        /// POST: api/Account/ConfirmPhoneNumber
        /// Confirm phoneNumber.
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberRequest request)
        {
            await _accountService.ConfirmPhoneNumberAsync(userName: request.UserName, token: request.Token);
            return Ok();
        }

        /// POST: api/Account/Login
        /// Return jwt token
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var jwtToken = await _accountService.AuthenticateAsync(request.Username, request.Password);
            return Ok(jwtToken);
        }

        /// POST: api/Account/ChangePassword
        /// Change password.
        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            await _accountService.ChangePasswordAsync(currentPassword: request.CurrentPassword, newPassword: request.NewPassword);
            return Ok();
        }

        /// POST: api/Account/GenerateChangeEmailToken
        /// Generate changeEmailToken and send it to NewEmail.
        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult<string>> GenerateChangeEmailToken(GenerateChangeEmailTokenRequest request)
        {
            var token = await _accountService.GenerateChangeEmailTokenAsync(newEmail: request.NewEmail);
            //DOTO: must sent token to NewEmail
            return Ok(token);
        }

        /// POST: api/Account/GenerateChangePhoneNumberToken
        /// Generate changePhoneNumberToken and send it to NewPhoneNumber.
        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult> GenerateChangePhoneNumberToken(GenerateChangePhoneNumberTokenRequest request)
        {
            var token = await _accountService.GenerateChangePhoneNumberTokenAsync(newPhoneNumber: request.NewPhoneNumber);
            //DOTO: must sent token to NewPhoneNumber
            return Ok(token);
        }

        /// POST: api/Account/ChangeEmail
        /// Change email.
        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult> ChangeEmail(ChangeEmailRequest request)
        {
            await _accountService.ChangeEmailAsync(newEmail: request.NewEmail, token: request.Token);
            return Ok();
        }

        /// POST: api/Account/ChangePhoneNumber
        /// Change phoneNumber.
        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult> ChangePhoneNumber(ChangePhoneNumberRequest request)
        {
            await _accountService.ChangePhoneNumberAsync(newPhoneNumber: request.NewPhoneNumber, token: request.Token);
            return Ok();
        }


        /// POST: api/Account/GeneratePasswordResetToken
        /// Generate passwordResetToken and send it to Email or PhoneNumber.
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> GeneratePasswordResetToken(GeneratePasswordResetTokenRequest request)
        {
            var token = await _accountService.GeneratePasswordResetTokenAsync(userName: request.UserName);
            // TODO: must send token to Email or PhoneNumber
            return Ok(token);
        }

        /// Get: api/Account/ResetPassword
        /// change password to new password.
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await _accountService.ResetPasswordAsync(userName: request.UserName, token: request.Token, newPassword: request.NewPassword);
            return Ok();
        }
    }
}
