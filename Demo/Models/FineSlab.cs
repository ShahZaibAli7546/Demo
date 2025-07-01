using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class FineSlab
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Select Mode is required")]
        [Display(Name = "Select Mode")]
        public string SelectMode { get; set; } = string.Empty;

        [Display(Name = "Applied Days")]
        public int? AppliedDays { get; set; }

        [Required(ErrorMessage = "Fine Value is required")]
        [Display(Name = "Fine Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid amount")]
        public decimal FineValue { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
    }
}
