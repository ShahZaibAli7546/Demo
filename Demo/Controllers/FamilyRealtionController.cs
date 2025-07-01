using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Demo.Models;

namespace Demo.Controllers
{
    public class FamilyRelationController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            var list = new List<FamilyRelation>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM FamilyRelation", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new FamilyRelation
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    RelationName = reader["RelationName"].ToString() ?? "",
                    Status = reader["Status"].ToString() ?? "Active"
                });
            }
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FamilyRelation model)
        {
            if (!ModelState.IsValid) return View(model);
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("INSERT INTO FamilyRelation (RelationName, Status) VALUES (@Name, @Status)", conn);
            cmd.Parameters.AddWithValue("@Name", model.RelationName);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
            TempData["SuccessMessage"] = "Relation added successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            FamilyRelation? model = null;
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM FamilyRelation WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new FamilyRelation
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    RelationName = reader["RelationName"].ToString() ?? "",
                    Status = reader["Status"].ToString() ?? "Active"
                };
            }
            return model is null ? NotFound() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FamilyRelation model)
        {
            if (!ModelState.IsValid) return View(model);
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE FamilyRelation SET RelationName = @Name, Status = @Status WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Name", model.RelationName);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
            TempData["SuccessMessage"] = "Relation updated successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            FamilyRelation? model = null;
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM FamilyRelation WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new FamilyRelation
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    RelationName = reader["RelationName"].ToString() ?? "",
                    Status = reader["Status"].ToString() ?? "Active"
                };
            }
            return model is null ? NotFound() : View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM FamilyRelation WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
            TempData["SuccessMessage"] = "Relation deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
