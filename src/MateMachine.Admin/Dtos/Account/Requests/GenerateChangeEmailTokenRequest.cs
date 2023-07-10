using System.ComponentModel.DataAnnotations;

namespace MateMachine.Dto.Account.Requests
{
    public class GenerateChangeEmailTokenRequest
    {
        [Required]
        [EmailAddress]
        [Display(Name = "NewEmail")]
        public string NewEmail { get; set; }
    }
}
