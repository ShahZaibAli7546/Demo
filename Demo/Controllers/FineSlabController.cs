using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class FineSlabController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        public IActionResult Index()
        {
            var list = new List<FineSlab>();

            using SqlConnection con = new SqlConnection(ConnectionString);
            string query = "SELECT * FROM FineSlabs";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new FineSlab
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Title = reader["Title"].ToString() ?? "",
                    SelectMode = reader["SelectMode"].ToString() ?? "",
                    AppliedDays = reader["AppliedDays"] != DBNull.Value ? Convert.ToInt32(reader["AppliedDays"]) : null,
                    FineValue = Convert.ToDecimal(reader["FineValue"]),
                    Status = reader["Status"].ToString() ?? "Active"
                });
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View(new FineSlab()); // ✅ Always pass a new instance
        }

        [HttpPost]
        public IActionResult Create(FineSlab model)
        {
            if (ModelState.IsValid)
            {
                using SqlConnection con = new SqlConnection(ConnectionString);
                string query = @"INSERT INTO FineSlabs (Title, SelectMode, AppliedDays, FineValue, Status)
                                 VALUES (@Title, @SelectMode, @AppliedDays, @FineValue, @Status)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@SelectMode", model.SelectMode);
                cmd.Parameters.AddWithValue("@AppliedDays", (object?)model.AppliedDays ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FineValue", model.FineValue);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = "Fine Slab created successfully.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            FineSlab model = new();

            using SqlConnection con = new SqlConnection(ConnectionString);
            string query = "SELECT * FROM FineSlabs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Title = reader["Title"].ToString() ?? "";
                model.SelectMode = reader["SelectMode"].ToString() ?? "";
                model.AppliedDays = reader["AppliedDays"] != DBNull.Value ? Convert.ToInt32(reader["AppliedDays"]) : null;
                model.FineValue = Convert.ToDecimal(reader["FineValue"]);
                model.Status = reader["Status"].ToString() ?? "Active";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(FineSlab model)
        {
            if (ModelState.IsValid)
            {
                using SqlConnection con = new SqlConnection(ConnectionString);
                string query = @"UPDATE FineSlabs 
                                 SET Title = @Title, SelectMode = @SelectMode, AppliedDays = @AppliedDays,
                                     FineValue = @FineValue, Status = @Status
                                 WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@SelectMode", model.SelectMode);
                cmd.Parameters.AddWithValue("@AppliedDays", (object?)model.AppliedDays ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FineValue", model.FineValue);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                cmd.ExecuteNonQuery();

                TempData["SuccessMessage"] = "Fine Slab updated successfully.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            string query = "DELETE FROM FineSlabs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Fine Slab deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
