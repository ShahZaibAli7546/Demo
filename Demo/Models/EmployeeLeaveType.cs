using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class EmployeeLeaveType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Leave Type Name")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Background Color")]
        public string BackgroundColor { get; set; } = "#ffffff";

        [Display(Name = "Symbol")]
        public string Symbol { get; set; } = "🗓️";

        [Required]
        [Display(Name = "Weightage")]
        public string Weightage { get; set; } = "Full"; // options: Full, Half, etc.

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // options: Active, Inactive
    }
}
