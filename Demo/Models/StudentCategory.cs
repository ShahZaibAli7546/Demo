using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class StudentCategory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Default: Active
    }
}
