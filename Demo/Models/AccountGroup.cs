using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // ✅ Required for validation attributes

namespace Demo.Models
{
    public class AccountGroup
    {
        public int Id { get; set; }

        [Required]
        public string GroupCode { get; set; } = string.Empty;

        [Required]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        public string StatementType { get; set; } = string.Empty; // "BalanceSheet" or "ProfitAndLoss"

        public bool IsActive { get; set; } = true;

        public List<Account>? Accounts { get; set; }
    }
}
