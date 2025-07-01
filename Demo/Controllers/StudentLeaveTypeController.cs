using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Demo.Models;

namespace Demo.Controllers
{
    public class StudentLeaveTypeController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            var list = new List<StudentLeaveType>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentLeaveType", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new StudentLeaveType
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? "",
                    BackgroundColor = reader["BackgroundColor"]?.ToString() ?? "#ffffff",
                    Symbol = reader["Symbol"]?.ToString() ?? "📝",
                    Status = reader["Status"]?.ToString() ?? "Active"
                });
            }

            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentLeaveType model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("INSERT INTO StudentLeaveType (LeaveTypeName, BackgroundColor, Symbol, Status) VALUES (@Name, @Color, @Symbol, @Status)", conn);
            cmd.Parameters.AddWithValue("@Name", model.LeaveTypeName);
            cmd.Parameters.AddWithValue("@Color", model.BackgroundColor);
            cmd.Parameters.AddWithValue("@Symbol", model.Symbol);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Leave type added successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            StudentLeaveType? model = null;

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentLeaveType WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new StudentLeaveType
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? "",
                    BackgroundColor = reader["BackgroundColor"]?.ToString() ?? "#ffffff",
                    Symbol = reader["Symbol"]?.ToString() ?? "📝",
                    Status = reader["Status"]?.ToString() ?? "Active"
                };
            }

            return model is null ? NotFound() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentLeaveType model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE StudentLeaveType SET LeaveTypeName = @Name, BackgroundColor = @Color, Symbol = @Symbol, Status = @Status WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Name", model.LeaveTypeName);
            cmd.Parameters.AddWithValue("@Color", model.BackgroundColor);
            cmd.Parameters.AddWithValue("@Symbol", model.Symbol);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Leave type updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            StudentLeaveType? model = null;

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentLeaveType WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new StudentLeaveType
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? "",
                    BackgroundColor = reader["BackgroundColor"]?.ToString() ?? "#ffffff",
                    Symbol = reader["Symbol"]?.ToString() ?? "📝",
                    Status = reader["Status"]?.ToString() ?? "Active"
                };
            }

            return model is null ? NotFound() : View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM StudentLeaveType WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Leave type deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
