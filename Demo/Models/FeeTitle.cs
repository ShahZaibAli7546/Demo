using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class FeeTitle
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fee Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";  // Active or Inactive
    }
}
