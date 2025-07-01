using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class ChartOfAccount
    {
        public int AccountId { get; set; }

        // 🔹 Common Field
        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        // 🔹 Used for both group & head
        [Display(Name = "Parent Account (Group)")]
        public int? ParentAccountId { get; set; } // NULL = it's a group

        // 🔹 Group Head (Only for group)
        [Display(Name = "Group Head")]
        public string? GroupHead { get; set; } // Assets, Liabilities, Equity

        // 🔹 Account Type (Only for head)
        [Display(Name = "Account Type")]
        public string? AccountType { get; set; } // Debit or Credit

        // 🔹 Starting balance (Only for head)
        [Display(Name = "Starting Balance")]
        [Range(0, double.MaxValue, ErrorMessage = "Starting balance must be non-negative.")]
        public decimal? StartingBalance { get; set; }

        // 🔹 Status (Common)
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";

        // 🔹 Extra (for dropdown binding)
        public string? ParentAccountName { get; set; }
    }
}
