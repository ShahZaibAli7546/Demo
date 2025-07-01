using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class CapitalSubAccount
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
    }
}
