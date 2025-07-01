using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class EmployeeWithdrawStatus
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Withdraw Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Withdraw Reason")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Withdrawn";
    }
}
