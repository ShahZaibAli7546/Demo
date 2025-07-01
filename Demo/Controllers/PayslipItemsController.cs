using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class PayslipItemsController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            var model = new PayslipItemsViewModel
            {
                EarningItems = GetEarningItems(),
                DeductionItems = GetDeductionItems(),
                ContributionItems = GetContributionItems()
            };
            return View(model);
        }

        private List<PayslipEarningItem> GetEarningItems()
        {
            var items = new List<PayslipEarningItem>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT e.Id, e.Title, e.ExpenseAccountId, a.Title AS ExpenseAccountName FROM PayslipEarningItems e JOIN Accounts a ON e.ExpenseAccountId = a.Id", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new PayslipEarningItem
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    ExpenseAccountId = (int)reader["ExpenseAccountId"],
                    ExpenseAccountName = reader["ExpenseAccountName"].ToString()!
                });
            }
            return items;
        }

        private List<PayslipDeductionItem> GetDeductionItems()
        {
            var items = new List<PayslipDeductionItem>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT d.Id, d.Title, d.ExpenseAccountId, d.LiabilityAccountId, d.IsAttendance, d.IsPayable, ea.Title AS ExpenseAccountName, la.Title AS LiabilityAccountName FROM PayslipDeductionItems d JOIN Accounts ea ON d.ExpenseAccountId = ea.Id JOIN Accounts la ON d.LiabilityAccountId = la.Id", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new PayslipDeductionItem
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    ExpenseAccountId = (int)reader["ExpenseAccountId"],
                    LiabilityAccountId = (int)reader["LiabilityAccountId"],
                    IsAttendance = (bool)reader["IsAttendance"],
                    IsPayable = (bool)reader["IsPayable"],
                    ExpenseAccountName = reader["ExpenseAccountName"].ToString()!,
                    LiabilityAccountName = reader["LiabilityAccountName"].ToString()!
                });
            }
            return items;
        }

        private List<PayslipContributionItem> GetContributionItems()
        {
            var items = new List<PayslipContributionItem>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT c.Id, c.Title, c.ExpenseAccountId, c.LiabilityAccountId, ea.Title AS ExpenseAccountName, la.Title AS LiabilityAccountName FROM PayslipContributionItems c JOIN Accounts ea ON c.ExpenseAccountId = ea.Id JOIN Accounts la ON c.LiabilityAccountId = la.Id", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new PayslipContributionItem
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    ExpenseAccountId = (int)reader["ExpenseAccountId"],
                    LiabilityAccountId = (int)reader["LiabilityAccountId"],
                    ExpenseAccountName = reader["ExpenseAccountName"].ToString()!,
                    LiabilityAccountName = reader["LiabilityAccountName"].ToString()!
                });
            }
            return items;
        }
    }
}