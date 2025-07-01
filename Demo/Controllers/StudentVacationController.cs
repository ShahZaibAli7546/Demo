using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class StudentVacationController : Controller
    {
        private readonly IConfiguration _config;

        public StudentVacationController(IConfiguration config)
        {
            _config = config;
        }

        // GET: /StudentVacation
        public IActionResult Index()
        {
            List<StudentVacation> vacations = new();
            string connStr = _config.GetConnectionString("DefaultConnection")!;
            using var conn = new SqlConnection(connStr);
            string query = "SELECT * FROM StudentVacations ORDER BY StartDate DESC";

            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                vacations.Add(new StudentVacation
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    VacationTitle = reader["VacationTitle"]?.ToString() ?? "",
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    Description = reader["Description"]?.ToString() ?? "",
                    BackgroundColor = reader["BackgroundColor"]?.ToString() ?? "",
                    Symbol = reader["Symbol"]?.ToString() ?? "",
                    Campus = reader["Campus"]?.ToString() ?? "",
                    Status = reader["Status"]?.ToString() ?? "",
                    ApplyOnWeekend = Convert.ToBoolean(reader["ApplyOnWeekend"]),
                    Batch = reader["Batch"]?.ToString() ?? ""
                });
            }

            return View(vacations);
        }

        // GET: /StudentVacation/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: /StudentVacation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentVacation vacation)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(vacation);
            }

            string connStr = _config.GetConnectionString("DefaultConnection")!;
            using var conn = new SqlConnection(connStr);

            string query = @"
                INSERT INTO StudentVacations 
                (VacationTitle, StartDate, EndDate, Description, BackgroundColor, Symbol, Campus, Status, ApplyOnWeekend, Batch)
                VALUES 
                (@VacationTitle, @StartDate, @EndDate, @Description, @BackgroundColor, @Symbol, @Campus, @Status, @ApplyOnWeekend, @Batch)";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@VacationTitle", vacation.VacationTitle);
            cmd.Parameters.AddWithValue("@StartDate", vacation.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", vacation.EndDate);
            cmd.Parameters.AddWithValue("@Description", vacation.Description ?? "");
            cmd.Parameters.AddWithValue("@BackgroundColor", vacation.BackgroundColor);
            cmd.Parameters.AddWithValue("@Symbol", vacation.Symbol ?? "");
            cmd.Parameters.AddWithValue("@Campus", vacation.Campus);
            cmd.Parameters.AddWithValue("@Status", vacation.Status);
            cmd.Parameters.AddWithValue("@ApplyOnWeekend", vacation.ApplyOnWeekend);
            cmd.Parameters.AddWithValue("@Batch", vacation.Batch);

            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Vacation record added successfully.";
            return RedirectToAction("Index");
        }

        // GET: /StudentVacation/Edit/5
        public IActionResult Edit(int id)
        {
            var vacation = GetVacationById(id);
            if (vacation == null) return NotFound();

            PopulateDropdowns();
            return View(vacation);
        }

        // POST: /StudentVacation/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentVacation vacation)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(vacation);
            }

            string connStr = _config.GetConnectionString("DefaultConnection")!;
            using var conn = new SqlConnection(connStr);

            string query = @"
                UPDATE StudentVacations SET
                    VacationTitle = @VacationTitle,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Description = @Description,
                    BackgroundColor = @BackgroundColor,
                    Symbol = @Symbol,
                    Campus = @Campus,
                    Status = @Status,
                    ApplyOnWeekend = @ApplyOnWeekend,
                    Batch = @Batch
                WHERE Id = @Id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@VacationTitle", vacation.VacationTitle);
            cmd.Parameters.AddWithValue("@StartDate", vacation.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", vacation.EndDate);
            cmd.Parameters.AddWithValue("@Description", vacation.Description ?? "");
            cmd.Parameters.AddWithValue("@BackgroundColor", vacation.BackgroundColor);
            cmd.Parameters.AddWithValue("@Symbol", vacation.Symbol ?? "");
            cmd.Parameters.AddWithValue("@Campus", vacation.Campus);
            cmd.Parameters.AddWithValue("@Status", vacation.Status);
            cmd.Parameters.AddWithValue("@ApplyOnWeekend", vacation.ApplyOnWeekend);
            cmd.Parameters.AddWithValue("@Batch", vacation.Batch);
            cmd.Parameters.AddWithValue("@Id", vacation.Id);

            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Vacation updated successfully.";
            return RedirectToAction("Index");
        }

        // GET: /StudentVacation/Details/5
        public IActionResult Details(int id)
        {
            var vacation = GetVacationById(id);
            if (vacation == null) return NotFound();

            return View(vacation);
        }

        // GET: /StudentVacation/Delete/5
        public IActionResult Delete(int id)
        {
            var vacation = GetVacationById(id);
            if (vacation == null) return NotFound();

            return View(vacation);
        }

        // POST: /StudentVacation/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string connStr = _config.GetConnectionString("DefaultConnection")!;
            using var conn = new SqlConnection(connStr);
            string query = "DELETE FROM StudentVacations WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Vacation deleted successfully.";
            return RedirectToAction("Index");
        }

        // Shared function to get vacation by ID
        private StudentVacation? GetVacationById(int id)
        {
            string connStr = _config.GetConnectionString("DefaultConnection")!;
            using var conn = new SqlConnection(connStr);
            string query = "SELECT * FROM StudentVacations WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new StudentVacation
            {
                Id = (int)reader["Id"],
                VacationTitle = reader["VacationTitle"]?.ToString() ?? "",
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                EndDate = Convert.ToDateTime(reader["EndDate"]),
                Description = reader["Description"]?.ToString() ?? "",
                BackgroundColor = reader["BackgroundColor"]?.ToString() ?? "",
                Symbol = reader["Symbol"]?.ToString() ?? "",
                Campus = reader["Campus"]?.ToString() ?? "",
                Status = reader["Status"]?.ToString() ?? "",
                ApplyOnWeekend = Convert.ToBoolean(reader["ApplyOnWeekend"]),
                Batch = reader["Batch"]?.ToString() ?? ""
            };
        }

        // Helper: Populate ViewBag dropdowns
        private void PopulateDropdowns()
        {
            ViewBag.Campuses = new List<string> { "Main Campus", "Girls Campus", "Boys Campus" };
            ViewBag.Statuses = new List<string> { "Active", "Inactive" };
            ViewBag.Batches = new List<string> { "2021", "2022", "2023", "2024" };
            ViewBag.Colors = new List<string>
            {
                "#ffffff", "#f44336", "#4CAF50", "#2196F3", "#FF9800", "#9C27B0", "#607D8B"
            };
        }
    }
}
