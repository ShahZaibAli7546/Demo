using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class RecurringPayslip
    {
        public int Id { get; set; }

        [Display(Name = "Campus")]
        public int CampusId { get; set; }

        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Display(Name = "Rate Type")]
        public string RateType { get; set; } = string.Empty; // Per Day, Per Hour

        [Display(Name = "Loan Return")]
        public bool LoanReturn { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal GrossPay { get; set; } // Auto calculated
        public decimal TotalDeduction { get; set; } // Auto calculated
        public decimal TotalContribution { get; set; } // ✅ NEW
        public decimal NetPay => GrossPay - TotalDeduction; // Optional: Update if needed to subtract contributions too

        public List<RecurringPayslipEarningItem> EarningItems { get; set; } = new();
        public List<RecurringPayslipDeductionItem> DeductionItems { get; set; } = new();
        public List<RecurringPayslipContributionItem> ContributionItems { get; set; } = new();

        // ✅ ADD THESE TWO PROPERTIES for display in the View
        public string CampusName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
    }

    public class RecurringPayslipEarningItem
    {
        public int Id { get; set; }
        public int RecurringPayslipId { get; set; }

        [Display(Name = "Earning Item")]
        public int EarningItemId { get; set; }

        [Display(Name = "Earning Account")]
        public int EarningAccountId { get; set; }

        public decimal Rate { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class RecurringPayslipDeductionItem
    {
        public int Id { get; set; }
        public int RecurringPayslipId { get; set; }

        [Display(Name = "Deduction Item")]
        public int DeductionItemId { get; set; }

        [Display(Name = "Deduction Account")]
        public int DeductionAccountId { get; set; }

        public string Description { get; set; } = string.Empty;

        [Display(Name = "Deduction Type")]
        public string DeductionType { get; set; } = "Absolute";

        public decimal Amount { get; set; }
    }

    public class RecurringPayslipContributionItem
    {
        public int Id { get; set; }

        public int RecurringPayslipId { get; set; }

        [Display(Name = "Contribution Item")]
        public int ContributionItemId { get; set; }

        [Display(Name = "Contribution Account")]
        public int ContributionAccountId { get; set; }

        public string Description { get; set; } = string.Empty;

        [Display(Name = "Contribution Type")]
        public string ContributionType { get; set; } = "Absolute"; // "Absolute" or "Percentage"

        public decimal Amount { get; set; }

        [Display(Name = "Applicable Every Month")]
        public bool IsApplicableEveryMonth { get; set; } = true;
    }
}
