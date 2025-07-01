using System.Collections.Generic;

namespace Demo.Models
{
    public class ChartOfAccountViewModel
    {
        public ChartOfAccount NewGroup { get; set; } = new();
        public ChartOfAccount NewAccount { get; set; } = new();
        public List<ChartOfAccount> ParentAccounts { get; set; } = new();
    }
}
