using System.ComponentModel.DataAnnotations;

namespace FeedBack_APP.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User is required.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
