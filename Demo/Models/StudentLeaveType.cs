using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class StudentLeaveType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public string LeaveTypeName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Background Color")]
        public string BackgroundColor { get; set; } = "#ffffff";

        [Required]
        [Display(Name = "Symbol")]
        public string Symbol { get; set; } = "📝";

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Default: Active
    }
}
