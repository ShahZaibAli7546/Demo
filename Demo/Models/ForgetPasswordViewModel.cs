using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class ForgotPasswordViewModel
    {
        // Input field used for identifying user (can be username or email)
        [Required(ErrorMessage = "Please enter your email or username")]
        [Display(Name = "Email or Username")]
        public string Identifier { get; set; } = string.Empty;

        // These are only used during reset, not when requesting the link
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Please enter a new password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        public string? Token { get; set; }
    }
}
