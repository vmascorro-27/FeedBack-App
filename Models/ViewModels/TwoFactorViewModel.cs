using System.ComponentModel.DataAnnotations;

namespace FeedBack_APP.Models.ViewModels
{
    public class TwoFactorViewModel
    {
        [Required(ErrorMessage = "The authentication code is required.")]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string MaskedEmail { get; set; } = string.Empty;
        public string ManualEntryKey { get; set; } = string.Empty;
        public string QrCodeImageUrl { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
    }
}
