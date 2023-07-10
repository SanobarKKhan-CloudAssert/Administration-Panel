using System.ComponentModel.DataAnnotations;

namespace MateMachine.Dto.Account.Requests
{
    public class ChangeEmailRequest
    {
        [Required]
        [EmailAddress]
        [Display(Name = "NewEmail")]
        public string NewEmail { get; set; }

        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }
    }
}
