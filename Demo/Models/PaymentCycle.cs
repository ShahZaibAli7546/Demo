using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class PaymentCycle
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Payment Cycle Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 12, ErrorMessage = "Value must be between 1 and 12")]
        [Display(Name = "Value (e.g., months)")]
        public int Value { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
    }
}
