using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class UpdateTransportFee
    {
        public int Id { get; set; }

        [Required]
        public int CampusId { get; set; }

        [Required]
        public int BatchId { get; set; }

        [Required]
        public int FeeServiceId { get; set; }

        [Required]
        [Display(Name = "Select Type")]
        public string SelectType { get; set; } = string.Empty; // Amount Base / Percentage Base

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Status { get; set; } = "Active";

        // 🟦 Optional display properties (filled in controller manually)
        public string CampusName { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public string FeeServiceName { get; set; } = string.Empty;
    }
}
