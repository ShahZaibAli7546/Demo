using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class TakePaymentReceiptController : Controller
    {
        private readonly string _connectionString;

        public TakePaymentReceiptController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult PrintReceipt(string invoiceNo)
        {
            var model = new TakePaymentReceiptFormat();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            // 🏫 School Info
            using (var cmd = new SqlCommand("SELECT TOP 1 SchoolName, Logo1Path FROM SchoolGeneralSettings", con))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    model.SchoolName = reader["SchoolName"]?.ToString();
                    model.SchoolLogoPath = reader["Logo1Path"]?.ToString();
                }
            }

            // 📄 Voucher + Student Info
            using (var cmd = new SqlCommand(@"
                SELECT s.AdmissionNo, s.StudentName, s.FatherName, b.BatchName,
                       v.Month, v.InvoiceNo, v.PaymentDate, a.AccountName,
                       v.TotalFine, v.TotalCredit, v.LastPaidAmount
                FROM FeeVoucher v
                INNER JOIN Students s ON v.StudentId = s.StudentId
                INNER JOIN Batches b ON s.BatchId = b.BatchId
                INNER JOIN Accounts a ON v.AccountId = a.AccountId
                WHERE v.InvoiceNo = @InvoiceNo", con))
            {
                cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.AdmissionNo = reader["AdmissionNo"]?.ToString();
                    model.StudentName = reader["StudentName"]?.ToString();
                    model.FatherName = reader["FatherName"]?.ToString();
                    model.BatchName = reader["BatchName"]?.ToString();
                    model.Month = reader["Month"]?.ToString();
                    model.InvoiceNo = reader["InvoiceNo"]?.ToString();
                    model.PaymentDate = reader["PaymentDate"] as DateTime?;
                    model.AccountName = reader["AccountName"]?.ToString();
                    model.TotalFine = Convert.ToDecimal(reader["TotalFine"]);
                    model.TotalCredit = Convert.ToDecimal(reader["TotalCredit"]);
                    model.LastPaidAmount = Convert.ToDecimal(reader["LastPaidAmount"]);
                }
            }

            // 💵 Fee Details
            using (var cmd = new SqlCommand(@"
                SELECT fs.FeeServiceName, d.Amount, d.Discount
                FROM FeeVoucherDetails d
                INNER JOIN FeeServices fs ON d.FeeServiceId = fs.FeeServiceId
                WHERE d.InvoiceNo = @InvoiceNo", con))
            {
                cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                using var reader = cmd.ExecuteReader();
                int sr = 1;
                while (reader.Read())
                {
                    model.FeeDetails.Add(new TakePaymentReceiptFormat.ReceiptFeeDetail
                    {
                        SrNo = sr++,
                        ServiceName = reader["FeeServiceName"]?.ToString(),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        Discount = Convert.ToDecimal(reader["Discount"])
                    });
                }
            }

            return View("PrintReceipt", model);
        }
    }
}
