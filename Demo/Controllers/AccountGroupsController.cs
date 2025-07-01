using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers
{
    public class AccountGroupsController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(AccountGroup model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO AccountGroups (GroupCode, GroupName, StatementType, IsActive)
                VALUES (@GroupCode, @GroupName, @StatementType, @IsActive)", con);

            cmd.Parameters.AddWithValue("@GroupCode", model.GroupCode);
            cmd.Parameters.AddWithValue("@GroupName", model.GroupName);
            cmd.Parameters.AddWithValue("@StatementType", model.StatementType);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Group created successfully.";
            return RedirectToAction("Index", "AccountsDisplay");
        }

        public IActionResult Edit(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM AccountGroups WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return NotFound();

            var group = new AccountGroup
            {
                Id = (int)reader["Id"],
                GroupCode = reader["GroupCode"].ToString()!,
                GroupName = reader["GroupName"].ToString()!,
                StatementType = reader["StatementType"].ToString()!,
                IsActive = (bool)reader["IsActive"]
            };
            return View(group);
        }

        [HttpPost]
        public IActionResult Edit(AccountGroup model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE AccountGroups SET 
                    GroupCode = @GroupCode, 
                    GroupName = @GroupName, 
                    StatementType = @StatementType, 
                    IsActive = @IsActive 
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@GroupCode", model.GroupCode);
            cmd.Parameters.AddWithValue("@GroupName", model.GroupName);
            cmd.Parameters.AddWithValue("@StatementType", model.StatementType);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Group updated.";
            return RedirectToAction("Index", "AccountsDisplay");
        }

        public IActionResult Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM AccountGroups WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Group deleted.";
            return RedirectToAction("Index", "AccountsDisplay");
        }
    }
}
