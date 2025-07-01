using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class AccountsController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // 🔹 INDEX
        public IActionResult Index()
        {
            var accounts = new List<Account>();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM Accounts", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                accounts.Add(new Account
                {
                    Id = (int)reader["Id"],
                    AccountCode = reader["AccountCode"].ToString()!,
                    Title = reader["Title"].ToString()!,
                    IsActive = (bool)reader["IsActive"],
                    GroupId = (int)reader["GroupId"]
                });
            }

            return View(accounts);
        }

        // 🔹 CREATE (GET)
        public IActionResult Create()
        {
            ViewBag.Groups = GetGroupSelectList();
            return View();
        }

        // 🔹 CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Account account)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = GetGroupSelectList();
                return View(account);
            }

            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand(@"INSERT INTO Accounts (AccountCode, Title, IsActive, GroupId) 
                                       VALUES (@AccountCode, @Title, @IsActive, @GroupId)", con);

            cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode);
            cmd.Parameters.AddWithValue("@Title", account.Title);
            cmd.Parameters.AddWithValue("@IsActive", account.IsActive);
            cmd.Parameters.AddWithValue("@GroupId", account.GroupId);

            cmd.ExecuteNonQuery();

            return RedirectToAction("Index", "AccountsDisplay");
        }

        // 🔹 EDIT (GET)
        public IActionResult Edit(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM Accounts WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();

            var account = new Account
            {
                Id = (int)reader["Id"],
                AccountCode = reader["AccountCode"].ToString()!,
                Title = reader["Title"].ToString()!,
                IsActive = (bool)reader["IsActive"],
                GroupId = (int)reader["GroupId"]
            };

            ViewBag.Groups = GetGroupSelectList();
            return View(account);
        }

        // 🔹 EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Account account)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = GetGroupSelectList();
                return View(account);
            }

            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand(@"UPDATE Accounts SET 
                AccountCode = @AccountCode,
                Title = @Title,
                IsActive = @IsActive,
                GroupId = @GroupId
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Id", account.Id);
            cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode);
            cmd.Parameters.AddWithValue("@Title", account.Title);
            cmd.Parameters.AddWithValue("@IsActive", account.IsActive);
            cmd.Parameters.AddWithValue("@GroupId", account.GroupId);

            cmd.ExecuteNonQuery();

            return RedirectToAction("Index", "AccountsDisplay");
        }

        // 🔹 DELETE (GET)
        public IActionResult Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM Accounts WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();

            var account = new Account
            {
                Id = (int)reader["Id"],
                AccountCode = reader["AccountCode"].ToString()!,
                Title = reader["Title"].ToString()!,
                IsActive = (bool)reader["IsActive"],
                GroupId = (int)reader["GroupId"]
            };

            return View(account);
        }

        // 🔹 DELETE CONFIRMED (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            // 🔒 Check if account is linked in FeeServices
            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM FeeServices WHERE AccountId = @Id", con);
            checkCmd.Parameters.AddWithValue("@Id", id);
            int usageCount = (int)checkCmd.ExecuteScalar();

            if (usageCount > 0)
            {
                TempData["ErrorMessage"] = "❌ Cannot delete: This account is linked to a Fee Service.";
                return RedirectToAction("Index", "AccountsDisplay");
            }

            // 🔄 Try deleting
            var deleteCmd = new SqlCommand("DELETE FROM Accounts WHERE Id = @Id", con);
            deleteCmd.Parameters.AddWithValue("@Id", id);

            int rowsAffected = deleteCmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                TempData["SuccessMessage"] = "✅ Account deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Deletion failed. Record may not exist.";
            }

            return RedirectToAction("Index", "AccountsDisplay");
        }

        // 🔹 HELPER: Load AccountGroups as SelectList
        private List<SelectListItem> GetGroupSelectList()
        {
            var list = new List<SelectListItem>();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand("SELECT Id, GroupName FROM AccountGroups WHERE IsActive = 1", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["Id"].ToString(),
                    Text = reader["GroupName"].ToString()
                });
            }

            return list;
        }
    }
}
