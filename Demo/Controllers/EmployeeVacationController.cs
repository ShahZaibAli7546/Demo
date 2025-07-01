using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class EmployeeVacationController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeeVacationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ Index
        public IActionResult Index()
        {
            List<EmployeeVacation> list = new();
            string connection = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new(connection);
            using SqlCommand cmd = new("SELECT * FROM EmployeeVacation", con);
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new EmployeeVacation
                {
                    Id = Convert.ToInt32(rdr["Id"]),
                    Title = rdr["Title"].ToString()!,
                    StartDate = Convert.ToDateTime(rdr["StartDate"]),
                    EndDate = Convert.ToDateTime(rdr["EndDate"]),
                    Description = rdr["Description"]?.ToString(),
                    BackgroundColor = rdr["BackgroundColor"]?.ToString(),
                    Symbol = rdr["Symbol"]?.ToString(),
                    ApplyOnWeekend = Convert.ToBoolean(rdr["ApplyOnWeekend"]),
                    Status = rdr["Status"].ToString()!
                });
            }

            return View(list);
        }

        // ✅ Create - GET
        public IActionResult Create() => View();

        // ✅ Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeVacation model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string query = @"INSERT INTO EmployeeVacation 
                            (Title, StartDate, EndDate, Description, BackgroundColor, Symbol, ApplyOnWeekend, Status) 
                            VALUES 
                            (@Title, @StartDate, @EndDate, @Description, @BackgroundColor, @Symbol, @ApplyOnWeekend, @Status)";

            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection")!);
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BackgroundColor", model.BackgroundColor ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Symbol", model.Symbol ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ApplyOnWeekend", model.ApplyOnWeekend);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Vacation added successfully!";
            return RedirectToAction("Index");
        }

        // ✅ Edit - GET
        public IActionResult Edit(int id)
        {
            EmployeeVacation model = new();
            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection")!);
            using SqlCommand cmd = new("SELECT * FROM EmployeeVacation WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                model.Id = Convert.ToInt32(rdr["Id"]);
                model.Title = rdr["Title"].ToString()!;
                model.StartDate = Convert.ToDateTime(rdr["StartDate"]);
                model.EndDate = Convert.ToDateTime(rdr["EndDate"]);
                model.Description = rdr["Description"]?.ToString();
                model.BackgroundColor = rdr["BackgroundColor"]?.ToString();
                model.Symbol = rdr["Symbol"]?.ToString();
                model.ApplyOnWeekend = Convert.ToBoolean(rdr["ApplyOnWeekend"]);
                model.Status = rdr["Status"]!.ToString()!;
            }
            return View(model);
        }

        // ✅ Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeVacation model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string query = @"UPDATE EmployeeVacation 
                            SET Title = @Title, StartDate = @StartDate, EndDate = @EndDate, 
                                Description = @Description, BackgroundColor = @BackgroundColor, 
                                Symbol = @Symbol, ApplyOnWeekend = @ApplyOnWeekend, Status = @Status 
                            WHERE Id = @Id";

            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection")!);
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BackgroundColor", model.BackgroundColor ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Symbol", model.Symbol ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ApplyOnWeekend", model.ApplyOnWeekend);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Vacation updated successfully!";
            return RedirectToAction("Index");
        }

        // ✅ Delete - GET
        public IActionResult Delete(int id)
        {
            EmployeeVacation model = new();
            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection")!);
            using SqlCommand cmd = new("SELECT * FROM EmployeeVacation WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                model.Id = Convert.ToInt32(rdr["Id"]);
                model.Title = rdr["Title"].ToString()!;
                model.StartDate = Convert.ToDateTime(rdr["StartDate"]);
                model.EndDate = Convert.ToDateTime(rdr["EndDate"]);
                model.Description = rdr["Description"]?.ToString();
                model.BackgroundColor = rdr["BackgroundColor"]?.ToString();
                model.Symbol = rdr["Symbol"]?.ToString();
                model.ApplyOnWeekend = Convert.ToBoolean(rdr["ApplyOnWeekend"]);
                model.Status = rdr["Status"].ToString()!;
            }
            return View(model);
        }

        // ✅ Delete - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string query = "DELETE FROM EmployeeVacation WHERE Id = @Id";
            using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection")!);
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Vacation deleted.";
            return RedirectToAction("Index");
        }
    }
}
