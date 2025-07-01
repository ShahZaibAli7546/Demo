using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class EmployeeBank
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; } = string.Empty;

        public string? BranchCode { get; set; }
    }
}
