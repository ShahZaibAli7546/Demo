using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Demo.Models;

namespace Demo.Controllers
{
    public class StudentCategoryController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            var list = new List<StudentCategory>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentCategory", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new StudentCategory
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CategoryName = reader["CategoryName"]?.ToString() ?? "",
                    Status = reader["Status"]?.ToString() ?? "Active"
                });
            }

            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentCategory model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("INSERT INTO StudentCategory (CategoryName, Status) VALUES (@Name, @Status)", conn);
            cmd.Parameters.AddWithValue("@Name", model.CategoryName);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Category added successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            StudentCategory? model = null;

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentCategory WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new StudentCategory
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CategoryName = reader["CategoryName"]?.ToString() ?? "",
                    Status = reader["Status"]?.ToString() ?? "Active"
                };
            }

            return model is null ? NotFound() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentCategory model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE StudentCategory SET CategoryName = @Name, Status = @Status WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Name", model.CategoryName);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            StudentCategory? model = null;

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM StudentCategory WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new StudentCategory
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CategoryName = reader["CategoryName"]?.ToString() ?? "",
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
            using var cmd = new SqlCommand("DELETE FROM StudentCategory WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
