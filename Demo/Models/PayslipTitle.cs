using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class PayslipTitle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Payslip Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
    }
}
