using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class CapitalSubAccountController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // Index
        public IActionResult Index()
        {
            var list = new List<CapitalSubAccount>();
            using var con = new SqlConnection(_connectionString);
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM CapitalSubAccounts", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CapitalSubAccount
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return View(list);
        }

        // Create GET
        public IActionResult Create() => View();

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CapitalSubAccount model)
        {
            if (!ModelState.IsValid) return View(model);

            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("INSERT INTO CapitalSubAccounts (Title, IsActive) VALUES (@Title, @IsActive)", con);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        // Edit GET
        public IActionResult Edit(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT * FROM CapitalSubAccounts WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return NotFound();

            return View(new CapitalSubAccount
            {
                Id = (int)reader["Id"],
                Title = reader["Title"].ToString()!,
                IsActive = (bool)reader["IsActive"]
            });
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CapitalSubAccount model)
        {
            if (!ModelState.IsValid) return View(model);

            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("UPDATE CapitalSubAccounts SET Title = @Title, IsActive = @IsActive WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        // Delete GET
        public IActionResult Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT * FROM CapitalSubAccounts WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return NotFound();

            return View(new CapitalSubAccount
            {
                Id = (int)reader["Id"],
                Title = reader["Title"].ToString()!,
                IsActive = (bool)reader["IsActive"]
            });
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("DELETE FROM CapitalSubAccounts WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}
