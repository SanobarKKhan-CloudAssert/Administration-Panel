using System.ComponentModel.DataAnnotations;

namespace MateMachine.Dto.Account.Requests
{
    public class GenerateChangePhoneNumberTokenRequest
    {
        [Required]
        [Phone]
        [Display(Name = "NewPhoneNumber")]
        public string NewPhoneNumber { get; set; }
    }
}
