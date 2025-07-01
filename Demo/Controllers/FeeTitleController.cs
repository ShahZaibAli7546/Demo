using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers
{
    public class FeeTitleController : Controller
    {
        private readonly string connectionString;

        public FeeTitleController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // ✅ Index
        public IActionResult Index()
        {
            List<FeeTitle> titles = [];

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT * FROM FeeTitle", con);
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                titles.Add(new FeeTitle
                {
                    Id = reader.GetInt32(0),
                    Title = reader["Title"]?.ToString() ?? "",
                    Remarks = reader["Remarks"]?.ToString(),
                    Status = reader["Status"]?.ToString() ?? "Active"
                });
            }

            return View(titles);
        }

        // ✅ Create - GET
        public IActionResult Create() => View();

        // ✅ Create - POST
        [HttpPost]
        public IActionResult Create(FeeTitle model)
        {
            if (!ModelState.IsValid) return View(model);

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO FeeTitle (Title, Remarks, Status) 
                VALUES (@Title, @Remarks, @Status)", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", model.Status);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Fee title added successfully.";
            return RedirectToAction("Index");
        }

        // ✅ Edit - GET
        public IActionResult Edit(int id)
        {
            FeeTitle? model = null;
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT * FROM FeeTitle WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new FeeTitle
                {
                    Id = reader.GetInt32(0),
                    Title = reader["Title"]?.ToString() ?? "",
                    Remarks = reader["Remarks"]?.ToString(),
                    Status = reader["Status"]?.ToString() ?? "Active"
                };
            }

            return model == null ? NotFound() : View(model);
        }

        // ✅ Edit - POST
        [HttpPost]
        public IActionResult Edit(FeeTitle model)
        {
            if (!ModelState.IsValid) return View(model);

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE FeeTitle 
                SET Title = @Title, Remarks = @Remarks, Status = @Status 
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Fee title updated successfully.";
            return RedirectToAction("Index");
        }

        // ✅ Delete - GET
        public IActionResult Delete(int id)
        {
            FeeTitle? model = null;
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT * FROM FeeTitle WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new FeeTitle
                {
                    Id = reader.GetInt32(0),
                    Title = reader["Title"]?.ToString() ?? "",
                    Remarks = reader["Remarks"]?.ToString(),
                    Status = reader["Status"]?.ToString() ?? "Active"
                };
            }

            return model == null ? NotFound() : View(model);
        }

        // ✅ Delete - POST
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("DELETE FROM FeeTitle WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Fee title deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
