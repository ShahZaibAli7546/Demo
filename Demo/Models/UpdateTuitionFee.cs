using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    /// <summary>
    /// Represents a single student's tuition fee entry.
    /// </summary>
    public class UpdateTuitionFee
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string FatherName { get; set; } = string.Empty;

        [Display(Name = "Fee Service")]
        public int? FeeServiceId { get; set; }

        [Display(Name = "Discount")]
        public int? DiscountId { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// ViewModel for updating tuition fees by batch and campus.
    /// </summary>
    public class TuitionFeeBatchViewModel
    {
        [Display(Name = "Campus")]
        [Required(ErrorMessage = "Please select a campus.")]
        public int SelectedCampusId { get; set; }

        [Display(Name = "Batch")]
        [Required(ErrorMessage = "Please select a batch.")]
        public int SelectedBatchId { get; set; }

        public int? GlobalFeeServiceId { get; set; }

        public int? GlobalDiscountId { get; set; }

        public List<UpdateTuitionFee> Students { get; set; } = new();
    }
}
