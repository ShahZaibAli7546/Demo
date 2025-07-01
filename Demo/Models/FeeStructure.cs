using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class FeeStructure
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Structure Name")]
        public string? StructureName { get; set; }

        [Required]
        [Display(Name = "Service")]
        public int FeeServiceId { get; set; }  // Updated

        [Required]
        [Display(Name = "Discount")]
        public int FeeDiscountId { get; set; }  // Updated

        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // "Active" or "Inactive"

        // 🆕 Display-only properties
        [Display(Name = "Service Name")]
        public string FeeServiceName { get; set; } = string.Empty; // Updated

        [Display(Name = "Discount Name")]
        public string FeeDiscountName { get; set; } = string.Empty; // Updated
    }
}
