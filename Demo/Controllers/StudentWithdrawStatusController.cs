using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class StudentWithdrawStatusController : Controller
    {
        private readonly string _connectionString;

        public StudentWithdrawStatusController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult Index()
        {
            var list = new List<StudentWithdrawStatus>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentWithdrawStatus", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new StudentWithdrawStatus
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    StatusName = reader["StatusName"].ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString()
                });
            }

            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentWithdrawStatus model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("INSERT INTO StudentWithdrawStatus (StatusName, Description) VALUES (@StatusName, @Description)", conn);
            cmd.Parameters.AddWithValue("@StatusName", model.StatusName);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Withdraw status created successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentWithdrawStatus WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return NotFound();

            var model = new StudentWithdrawStatus
            {
                Id = Convert.ToInt32(reader["Id"]),
                StatusName = reader["StatusName"].ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentWithdrawStatus model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE StudentWithdrawStatus SET StatusName = @StatusName, Description = @Description WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@StatusName", model.StatusName);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Withdraw status updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentWithdrawStatus WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return NotFound();

            var model = new StudentWithdrawStatus
            {
                Id = Convert.ToInt32(reader["Id"]),
                StatusName = reader["StatusName"].ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString()
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM StudentWithdrawStatus WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Withdraw status deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
