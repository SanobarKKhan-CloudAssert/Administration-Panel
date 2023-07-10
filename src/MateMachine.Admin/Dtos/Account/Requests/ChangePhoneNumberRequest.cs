using System.ComponentModel.DataAnnotations;

namespace MateMachine.Dto.Account.Requests
{
    public class ChangePhoneNumberRequest
    {
        [Required]
        [Phone]
        [Display(Name = "NewPhoneNumber")]
        public string NewPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }
    }
}
