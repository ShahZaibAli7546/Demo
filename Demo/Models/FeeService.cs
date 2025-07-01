using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class FeeService
    {
        public int FeeServiceId { get; set; }

        [Required(ErrorMessage = "Fee Service name is required.")]
        [Display(Name = "Fee Service Name")]
        public string FeeServiceName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Service Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Account is required.")]
        [Display(Name = "Account")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Cost is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a positive number.")]
        public decimal Cost { get; set; }

        [Display(Name = "Payment Cycle")]
        public string? PaymentCycle { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Show on Voucher")]
        public bool ShowOnVoucher { get; set; }

        [Display(Name = "Taxable")]
        public bool Taxable { get; set; }

        [Display(Name = "Is Royalty")]
        public bool IsRoyalty { get; set; }

        [Display(Name = "Behavioral Fine Applicable")]
        public bool IsBehavioralFine { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        // ✅ Add this for batch-wise dropdown loading
        [Display(Name = "Batch")]
        public int? BatchId { get; set; }

        // For display purposes in list views
        public string? CategoryName { get; set; }
        public string? AccountName { get; set; }
    }
}
