using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class DiscountCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category title is required")]
        [Display(Name = "Category Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Active / Inactive
    }
}
