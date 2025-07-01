using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers
{
    public class DesignationController : Controller
    {
        private readonly string connectionString;

        public DesignationController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // 🔵 INDEX
        public IActionResult Index()
        {
            var designations = new List<Designation>();

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Designation", con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                designations.Add(new Designation
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? "",
                    Status = reader["Status"].ToString() ?? "Active"
                });
            }

            return View(designations);
        }

        // 🔵 CREATE
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Designation model)
        {
            if (!ModelState.IsValid) return View(model);

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("INSERT INTO Designation (Name, Status) VALUES (@Name, @Status)", con);

            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Status", model.Status);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Designation added successfully.";
            return RedirectToAction("Index");
        }

        // 🔵 EDIT
        public IActionResult Edit(int id)
        {
            Designation model = new();

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Designation WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Name = reader["Name"].ToString() ?? "";
                model.Status = reader["Status"].ToString() ?? "Active";
            }
            else
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Designation model)
        {
            if (!ModelState.IsValid) return View(model);

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE Designation SET
                    Name = @Name,
                    Status = @Status
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Status", model.Status);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Designation updated successfully.";
            return RedirectToAction("Index");
        }

        // 🔵 DELETE
        public IActionResult Delete(int id)
        {
            Designation model = new();

            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Designation WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Name = reader["Name"].ToString() ?? "";
                model.Status = reader["Status"].ToString() ?? "Active";
            }
            else
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using var con = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("DELETE FROM Designation WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Designation deleted.";
            return RedirectToAction("Index");
        }
    }
}
