namespace Demo.Models
{
    public class AccountGroupWithAccountsViewModel
    {
        public int GroupId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string StatementType { get; set; } = "BalanceSheet";
        public bool IsActive { get; set; }

        public List<Account> Accounts { get; set; } = [];
    }
}
