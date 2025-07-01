namespace Demo.Models
{
    public class PayslipEarningItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ExpenseAccountId { get; set; }
        public string ExpenseAccountName { get; set; } = string.Empty;
    }

    public class PayslipDeductionItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ExpenseAccountId { get; set; }
        public int LiabilityAccountId { get; set; }
        public bool IsAttendance { get; set; }
        public bool IsPayable { get; set; }
        public string ExpenseAccountName { get; set; } = string.Empty;
        public string LiabilityAccountName { get; set; } = string.Empty;
    }

    public class PayslipContributionItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ExpenseAccountId { get; set; }
        public int LiabilityAccountId { get; set; }
        public string ExpenseAccountName { get; set; } = string.Empty;
        public string LiabilityAccountName { get; set; } = string.Empty;
    }

    public class PayslipItemsViewModel
    {
        public List<PayslipEarningItem> EarningItems { get; set; } = new();
        public List<PayslipDeductionItem> DeductionItems { get; set; } = new();
        public List<PayslipContributionItem> ContributionItems { get; set; } = new();
    }

}