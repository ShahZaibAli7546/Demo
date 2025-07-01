using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Account code is required.")]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account title is required.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // 🔗 Foreign Key to AccountGroup
        [Required]
        [Display(Name = "Account Group")]
        public int GroupId { get; set; }

        public AccountGroup? Group { get; set; } // Navigation property
    }
}
