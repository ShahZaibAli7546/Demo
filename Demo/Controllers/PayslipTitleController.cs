using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class PayslipTitleController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            var list = new List<PayslipTitle>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT * FROM PayslipTitles", con);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PayslipTitle
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    IsActive = (bool)reader["IsActive"]
                });
            }
            return View(list);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(PayslipTitle model)
        {
            if (!ModelState.IsValid) return View(model);
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("INSERT INTO PayslipTitles (Title, IsActive) VALUES (@Title, @IsActive)", con);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT * FROM PayslipTitles WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return NotFound();

            var model = new PayslipTitle
            {
                Id = (int)reader["Id"],
                Title = reader["Title"].ToString()!,
                IsActive = (bool)reader["IsActive"]
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(PayslipTitle model)
        {
            if (!ModelState.IsValid) return View(model);
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("UPDATE PayslipTitles SET Title = @Title, IsActive = @IsActive WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("DELETE FROM PayslipTitles WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }
    }
}
