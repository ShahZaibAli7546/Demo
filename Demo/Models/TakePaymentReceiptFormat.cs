namespace Demo.Models
{
    public class TakePaymentReceiptFormat
    {
        // 🔷 From SchoolGeneralSettings
        public string? SchoolName { get; set; }
        public string? SchoolLogoPath { get; set; }

        // 🔷 From Account Info
        public string? AccountName { get; set; }

        // 🔷 From Student Info
        public string? AdmissionNo { get; set; }
        public string? StudentName { get; set; }
        public string? FatherName { get; set; }
        public string? BatchName { get; set; }

        // 🔷 Voucher Info
        public string? Month { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? PaymentDate { get; set; }

        // 🔷 Fee Detail Breakdown
        public class ReceiptFeeDetail
        {
            public int SrNo { get; set; }
            public string? ServiceName { get; set; }
            public decimal Amount { get; set; }
            public decimal Discount { get; set; }
            public decimal NetAmount => Amount - Discount;
        }

        public List<ReceiptFeeDetail> FeeDetails { get; set; } = new();

        // 🔷 Totals
        public decimal TotalDebit => FeeDetails.Sum(x => x.Amount);
        public decimal TotalDiscount => FeeDetails.Sum(x => x.Discount);
        public decimal TotalFine { get; set; } = 0; // optional if applied
        public decimal TotalCredit { get; set; } = 0;
        public decimal LastPaidAmount { get; set; } = 0;

        public decimal NetPayable => TotalDebit - TotalDiscount + TotalFine;
        public decimal ClosingBalance => NetPayable - TotalCredit;
    }

}
