using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class CashAccount
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cash Account Name")]
        public string CashAccountName { get; set; } = string.Empty;

        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; } = 0;

        [Display(Name = "Transfer Full Amount")]
        public bool TransferFullAmount { get; set; }

        // Not stored in DB; computed/displayed
        [Display(Name = "Total Balance")]
        public decimal TotalBalance => OpeningBalance; // Later you can sum with other transactions
    }
}
