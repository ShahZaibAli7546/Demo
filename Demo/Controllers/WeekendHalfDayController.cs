using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace Demo.Controllers
{
    public class WeekendHalfDayController : Controller
    {
        private readonly string _connectionString;

        public WeekendHalfDayController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult Index()
        {
            List<WeekendHalfDaySetup> list = new();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT * FROM WeekendHalfDaySetup", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new WeekendHalfDaySetup
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    CampusId = reader.GetInt32(reader.GetOrdinal("CampusId")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    WeekendDays = JsonSerializer.Deserialize<List<DayOfWeek>>(reader["WeekendDays"]?.ToString() ?? "[]") ?? new(),
                    HalfDayWeekdays = JsonSerializer.Deserialize<List<DayOfWeek>>(reader["HalfDayWeekdays"]?.ToString() ?? "[]") ?? new(),
                    SelectedBatchIds = JsonSerializer.Deserialize<List<int>>(reader["SelectedBatchIds"]?.ToString() ?? "[]") ?? new()
                });
            }

            return View(list);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(WeekendHalfDaySetup model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"INSERT INTO WeekendHalfDaySetup 
                (CampusId, StartDate, EndDate, WeekendDays, HalfDayWeekdays, SelectedBatchIds) 
                VALUES 
                (@CampusId, @StartDate, @EndDate, @WeekendDays, @HalfDayWeekdays, @SelectedBatchIds)", con);
            con.Open();

            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@WeekendDays", JsonSerializer.Serialize(model.WeekendDays));
            cmd.Parameters.AddWithValue("@HalfDayWeekdays", JsonSerializer.Serialize(model.HalfDayWeekdays));
            cmd.Parameters.AddWithValue("@SelectedBatchIds", JsonSerializer.Serialize(model.SelectedBatchIds));

            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            WeekendHalfDaySetup? model = null;

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT * FROM WeekendHalfDaySetup WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new WeekendHalfDaySetup
                {
                    Id = id,
                    CampusId = reader.GetInt32(reader.GetOrdinal("CampusId")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    WeekendDays = JsonSerializer.Deserialize<List<DayOfWeek>>(reader["WeekendDays"]?.ToString() ?? "[]") ?? new(),
                    HalfDayWeekdays = JsonSerializer.Deserialize<List<DayOfWeek>>(reader["HalfDayWeekdays"]?.ToString() ?? "[]") ?? new(),
                    SelectedBatchIds = JsonSerializer.Deserialize<List<int>>(reader["SelectedBatchIds"]?.ToString() ?? "[]") ?? new()
                };
            }

            if (model == null) return NotFound();

            LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(WeekendHalfDaySetup model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"UPDATE WeekendHalfDaySetup SET 
                CampusId = @CampusId, StartDate = @StartDate, EndDate = @EndDate, 
                WeekendDays = @WeekendDays, HalfDayWeekdays = @HalfDayWeekdays, 
                SelectedBatchIds = @SelectedBatchIds 
                WHERE Id = @Id", con);
            con.Open();

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@WeekendDays", JsonSerializer.Serialize(model.WeekendDays));
            cmd.Parameters.AddWithValue("@HalfDayWeekdays", JsonSerializer.Serialize(model.HalfDayWeekdays));
            cmd.Parameters.AddWithValue("@SelectedBatchIds", JsonSerializer.Serialize(model.SelectedBatchIds));

            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("DELETE FROM WeekendHalfDaySetup WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        private void LoadDropdowns()
        {
            ViewBag.Campuses = GetCampuses();
            ViewBag.Batches = GetBatches();
        }

        private List<SelectListItem> GetCampuses()
        {
            List<SelectListItem> campuses = new();
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT CampusId, CampusName FROM Campuses", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                campuses.Add(new SelectListItem
                {
                    Value = reader["CampusId"].ToString(),
                    Text = reader["CampusName"].ToString()
                });
            }
            return campuses;
        }

        private List<SelectListItem> GetBatches()
        {
            List<SelectListItem> batches = new()
            {
                new SelectListItem { Value = "0", Text = "Collective" }
            };

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT BatchId, BatchName FROM Batches", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                batches.Add(new SelectListItem
                {
                    Value = reader["BatchId"].ToString(),
                    Text = reader["BatchName"].ToString()
                });
            }
            return batches;
        }
    }
}
