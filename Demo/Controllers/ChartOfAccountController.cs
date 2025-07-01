using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class ChartOfAccountController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // Load view
        public IActionResult Index()
        {
            var viewModel = new ChartOfAccountViewModel
            {
                ParentAccounts = GetParentAccounts()
            };

            ViewBag.Accounts = GetAllAccounts();
            return View(viewModel);
        }

        // Save Group
        [HttpPost]
        public IActionResult SaveGroup(ChartOfAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParentAccounts = GetParentAccounts();
                ViewBag.Accounts = GetAllAccounts();
                return View("Index", model);
            }

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO ChartOfAccounts (Title, GroupHead, Status)
                VALUES (@Title, @GroupHead, @Status)", con);

            cmd.Parameters.AddWithValue("@Title", model.NewGroup.Title);
            cmd.Parameters.AddWithValue("@GroupHead", model.NewGroup.GroupHead ?? "Assets");
            cmd.Parameters.AddWithValue("@Status", model.NewGroup.Status ?? "Active");

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Group created successfully.";
            return RedirectToAction("Index");
        }

        // Save Account
        [HttpPost]
        public IActionResult SaveAccount(ChartOfAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParentAccounts = GetParentAccounts();
                ViewBag.Accounts = GetAllAccounts();
                return View("Index", model);
            }

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO ChartOfAccounts 
                (Title, ParentAccountId, AccountType, StartingBalance, Status)
                VALUES 
                (@Title, @ParentAccountId, @AccountType, @StartingBalance, @Status)", con);

            cmd.Parameters.AddWithValue("@Title", model.NewAccount.Title);
            cmd.Parameters.AddWithValue("@ParentAccountId", (object?)model.NewAccount.ParentAccountId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AccountType", model.NewAccount.AccountType ?? "Debit");
            cmd.Parameters.AddWithValue("@StartingBalance", model.NewAccount.StartingBalance ?? 0);
            cmd.Parameters.AddWithValue("@Status", model.NewAccount.Status ?? "Active");

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Account created successfully.";
            return RedirectToAction("Index");
        }

        private List<ChartOfAccount> GetParentAccounts()
        {
            List<ChartOfAccount> list = [];
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT AccountId, Title FROM ChartOfAccounts WHERE ParentAccountId IS NULL AND GroupHead IS NOT NULL", con);
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ChartOfAccount
                {
                    AccountId = reader.GetInt32(0),
                    Title = reader.GetString(1)
                });
            }
            return list;
        }

        private List<ChartOfAccount> GetAllAccounts()
        {
            List<ChartOfAccount> list = [];
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM ChartOfAccounts", con);
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ChartOfAccount
                {
                    AccountId = Convert.ToInt32(reader["AccountId"]),
                    Title = reader["Title"]?.ToString() ?? "",
                    GroupHead = reader["GroupHead"]?.ToString(),
                    ParentAccountId = reader["ParentAccountId"] != DBNull.Value ? Convert.ToInt32(reader["ParentAccountId"]) : null,
                    AccountType = reader["AccountType"]?.ToString(),
                    StartingBalance = reader["StartingBalance"] != DBNull.Value ? Convert.ToDecimal(reader["StartingBalance"]) : null,
                    Status = reader["Status"]?.ToString() ?? "Inactive"
                });
            }
            return list;
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM ChartOfAccounts WHERE AccountId = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
