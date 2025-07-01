using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class FeeDiscount
    {
        public int FeeDiscountId { get; set; }

        [Required]
        [Display(Name = "Discount Title")]
        public string FeeDiscountName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Discount Category")]
        public int DiscountCategoryId { get; set; }

        [Required]
        [Display(Name = "Discount Type")]
        public string DiscountType { get; set; } = string.Empty; // "Percentage" or "Fixed Amount"

        [Range(0, 100)]
        [Display(Name = "Percentage (%)")]
        public decimal? Percentage { get; set; }

        [Range(0, 999999)]
        [Display(Name = "Fixed Amount")]
        public decimal? Amount { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        // Optional: For display
        public string? DiscountCategoryName { get; set; }
    }
}
