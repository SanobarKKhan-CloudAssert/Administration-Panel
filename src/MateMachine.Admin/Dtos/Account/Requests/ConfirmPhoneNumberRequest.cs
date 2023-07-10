using System.ComponentModel.DataAnnotations;

namespace MateMachine.Dto.Account.Requests
{
    public class ConfirmPhoneNumberRequest
    {
        [Required]
        [Display(Name = "UserName")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }
    }
}
