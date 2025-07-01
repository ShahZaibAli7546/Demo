using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class ServiceCategory
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Remarks (Optional)")]
        public string? Remarks { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;  // Active / Inactive
    }
}
