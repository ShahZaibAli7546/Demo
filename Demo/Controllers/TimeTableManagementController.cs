using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace Demo.Controllers
{
    public class TimeTableManagementController : Controller
    {
        private readonly string _connectionString;

        public TimeTableManagementController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ✅ INDEX
        public IActionResult Index()
        {
            var list = new List<TimeTableManagement>();
            var campusMap = GetCampusMap();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT * FROM TimeTableManagement", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int campusId = Convert.ToInt32(reader["CampusId"]);
                list.Add(new TimeTableManagement
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Title = reader["Title"].ToString()!,
                    CampusId = campusId,
                    CampusName = campusMap.TryGetValue(campusId, out string name) ? name : "",
                    OpeningTime = reader["OpeningTime"] != DBNull.Value ? (TimeSpan)reader["OpeningTime"] : default,
                    ClosingTime = reader["ClosingTime"] != DBNull.Value ? (TimeSpan)reader["ClosingTime"] : default,
                    IsWeekend = reader["IsWeekend"] != DBNull.Value && Convert.ToBoolean(reader["IsWeekend"]),
                    IsHalfDay = reader["IsHalfDay"] != DBNull.Value && Convert.ToBoolean(reader["IsHalfDay"]),
                    HalfDayWeek = reader["HalfDayWeek"] != DBNull.Value ? Enum.Parse<DayOfWeek>(reader["HalfDayWeek"].ToString()!) : null,
                    HalfDayClosingTime = reader["HalfDayClosingTime"] != DBNull.Value ? (TimeSpan?)reader["HalfDayClosingTime"] : null,
                    LateLimitMinutes = Convert.ToInt32(reader["LateLimitMinutes"]),
                    EarlyOutLimitMinutes = Convert.ToInt32(reader["EarlyOutLimitMinutes"]),
                    Weekends = reader["Weekends"]?.ToString() ?? "",
                    HalfDays = reader["HalfDays"]?.ToString() ?? ""
                });
            }

            ViewBag.AppliedList = GetAppliedTimeTables();
            return View(list);
        }

        // ✅ CREATE
        public IActionResult Create()
        {
            LoadDropdowns();
            return View(new TimeTableManagement());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TimeTableManagement model)
        {
            LoadDropdowns();

            var setup = GetWeekendHalfDaySetup(model.CampusId, DateTime.Today);
            if (setup == null)
            {
                ModelState.AddModelError("", "Weekend and Half Day setup not found.");
                return View(model);
            }

            model.ValidWeekendDays = setup.WeekendDays;
            model.ValidHalfDayWeekdays = setup.HalfDayWeekdays;

            if (model.IsWeekend && !model.ValidWeekendDays.Any())
                ModelState.AddModelError(nameof(model.IsWeekend), "No weekend days configured.");

            if (model.IsHalfDay && (model.HalfDayWeek == null || !model.ValidHalfDayWeekdays.Contains(model.HalfDayWeek.Value)))
                ModelState.AddModelError(nameof(model.HalfDayWeek), "Invalid half-day weekday.");

            if (!ModelState.IsValid) return View(model);

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"INSERT INTO TimeTableManagement
                (Title, CampusId, OpeningTime, ClosingTime, IsWeekend, IsHalfDay, HalfDayWeek, HalfDayClosingTime,
                 LateLimitMinutes, EarlyOutLimitMinutes, Weekends, HalfDays)
                 VALUES
                (@Title, @CampusId, @OpeningTime, @ClosingTime, @IsWeekend, @IsHalfDay, @HalfDayWeek, @HalfDayClosingTime,
                 @LateLimitMinutes, @EarlyOutLimitMinutes, @Weekends, @HalfDays)", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
            cmd.Parameters.AddWithValue("@OpeningTime", model.OpeningTime);
            cmd.Parameters.AddWithValue("@ClosingTime", model.ClosingTime);
            cmd.Parameters.AddWithValue("@IsWeekend", model.IsWeekend);
            cmd.Parameters.AddWithValue("@IsHalfDay", model.IsHalfDay);
            cmd.Parameters.AddWithValue("@HalfDayWeek", model.HalfDayWeek?.ToString() ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HalfDayClosingTime", model.HalfDayClosingTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LateLimitMinutes", model.LateLimitMinutes);
            cmd.Parameters.AddWithValue("@EarlyOutLimitMinutes", model.EarlyOutLimitMinutes);
            cmd.Parameters.AddWithValue("@Weekends", string.Join(",", model.ValidWeekendDays));
            cmd.Parameters.AddWithValue("@HalfDays", string.Join(",", model.ValidHalfDayWeekdays));

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        // ✅ EDIT
        public IActionResult Edit(int id)
        {
            TimeTableManagement? model = GetTimeTableById(id);
            if (model == null) return NotFound();

            var setup = GetWeekendHalfDaySetup(model.CampusId, DateTime.Today);
            if (setup != null)
            {
                model.ValidWeekendDays = setup.WeekendDays;
                model.ValidHalfDayWeekdays = setup.HalfDayWeekdays;
            }

            LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TimeTableManagement model)
        {
            LoadDropdowns();

            var setup = GetWeekendHalfDaySetup(model.CampusId, DateTime.Today);
            model.ValidWeekendDays = setup?.WeekendDays ?? new();
            model.ValidHalfDayWeekdays = setup?.HalfDayWeekdays ?? new();

            if (!ModelState.IsValid) return View(model);

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"UPDATE TimeTableManagement SET
                Title=@Title, CampusId=@CampusId, OpeningTime=@OpeningTime, ClosingTime=@ClosingTime,
                IsWeekend=@IsWeekend, IsHalfDay=@IsHalfDay, HalfDayWeek=@HalfDayWeek, HalfDayClosingTime=@HalfDayClosingTime,
                LateLimitMinutes=@LateLimitMinutes, EarlyOutLimitMinutes=@EarlyOutLimitMinutes,
                Weekends=@Weekends, HalfDays=@HalfDays WHERE Id=@Id", con);

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
            cmd.Parameters.AddWithValue("@OpeningTime", model.OpeningTime);
            cmd.Parameters.AddWithValue("@ClosingTime", model.ClosingTime);
            cmd.Parameters.AddWithValue("@IsWeekend", model.IsWeekend);
            cmd.Parameters.AddWithValue("@IsHalfDay", model.IsHalfDay);
            cmd.Parameters.AddWithValue("@HalfDayWeek", model.HalfDayWeek?.ToString() ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HalfDayClosingTime", model.HalfDayClosingTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LateLimitMinutes", model.LateLimitMinutes);
            cmd.Parameters.AddWithValue("@EarlyOutLimitMinutes", model.EarlyOutLimitMinutes);
            cmd.Parameters.AddWithValue("@Weekends", string.Join(",", model.ValidWeekendDays));
            cmd.Parameters.AddWithValue("@HalfDays", string.Join(",", model.ValidHalfDayWeekdays));

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        // ✅ DELETE
        public IActionResult Delete(int id)
        {
            var model = GetTimeTableById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("DELETE FROM TimeTableManagement WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(Index));
        }

        // ✅ APPLY TIMETABLE
        public IActionResult Apply()
        {
            ViewBag.TimeTables = GetTimeTables();
            ViewBag.Batches = GetBatches();
            ViewBag.Employees = GetEmployees();
            ViewBag.Campuses = GetCampuses();
            ViewBag.AppliedList = GetAppliedTimeTables();

            return View(new TimeTableManagement
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(TimeTableManagement model)
        {
            ViewBag.TimeTables = GetTimeTables();
            ViewBag.Batches = GetBatches();
            ViewBag.Employees = GetEmployees();
            ViewBag.Campuses = GetCampuses();

            if (!ModelState.IsValid) return View(model);

            if (string.IsNullOrWhiteSpace(model.SelectedBatchId) && string.IsNullOrWhiteSpace(model.SelectedEmployeeId))
            {
                ModelState.AddModelError("", "Please select a batch or employee.");
                return View(model);
            }

            string applyTo = !string.IsNullOrWhiteSpace(model.SelectedBatchId) ? "Student" : "Employee";
            string selectedId = model.SelectedBatchId ?? model.SelectedEmployeeId!;

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"INSERT INTO ApplyTimeTable
                (TimeTableId, ApplyTo, SelectedIds, StartDate, EndDate, CampusId)
                VALUES (@TimeTableId, @ApplyTo, @SelectedIds, @StartDate, @EndDate, @CampusId)", con);

            cmd.Parameters.AddWithValue("@TimeTableId", model.Id);
            cmd.Parameters.AddWithValue("@ApplyTo", applyTo);
            cmd.Parameters.AddWithValue("@SelectedIds", selectedId);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        // ✅ Edit Apply
        public IActionResult EditApply(int id)
        {
            TimeTableManagement? model = null;

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT * FROM ApplyTimeTable WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new TimeTableManagement
                {
                    Id = Convert.ToInt32(reader["TimeTableId"]),
                    CampusId = Convert.ToInt32(reader["CampusId"]),
                    SelectedBatchId = reader["ApplyTo"].ToString() == "Student" ? reader["SelectedIds"].ToString() : null,
                    SelectedEmployeeId = reader["ApplyTo"].ToString() == "Employee" ? reader["SelectedIds"].ToString() : null,
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"])
                };
            }

            ViewBag.TimeTables = GetTimeTables();
            ViewBag.Batches = GetBatches();
            ViewBag.Employees = GetEmployees();
            ViewBag.Campuses = GetCampuses();

            return View("Apply", model);
        }

        // ✅ Delete Apply
        public IActionResult DeleteApply(int id)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("DELETE FROM ApplyTimeTable WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(Index));
        }

        // ✅ AJAX DROPDOWNS
        [HttpGet]
        public JsonResult GetBatchesAndEmployeesByCampus(int campusId)
        {
            var batches = GetSelectList("SELECT BatchId, BatchName FROM Batches WHERE CampusId = @CampusId", "BatchId", "BatchName", campusId);
            var employees = GetSelectList("SELECT EmployeeId, EmployeeName FROM Employees WHERE CampusId = @CampusId", "EmployeeId", "EmployeeName", campusId);

            return Json(new { batches, employees });
        }

        // 🔧 HELPERS
        private void LoadDropdowns()
        {
            ViewBag.Campuses = GetCampuses();
        }

        private Dictionary<int, string> GetCampusMap()
        {
            return GetSelectList("SELECT CampusId, CampusName FROM Campuses", "CampusId", "CampusName")
                .ToDictionary(item => int.Parse(item.Value), item => item.Text);
        }

        private TimeTableManagement? GetTimeTableById(int id)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT * FROM TimeTableManagement WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new TimeTableManagement
                {
                    Id = id,
                    Title = reader["Title"].ToString()!,
                    CampusId = Convert.ToInt32(reader["CampusId"]),
                    OpeningTime = (TimeSpan)reader["OpeningTime"],
                    ClosingTime = (TimeSpan)reader["ClosingTime"],
                    IsWeekend = Convert.ToBoolean(reader["IsWeekend"]),
                    IsHalfDay = Convert.ToBoolean(reader["IsHalfDay"]),
                    HalfDayWeek = reader["HalfDayWeek"] != DBNull.Value ? Enum.Parse<DayOfWeek>(reader["HalfDayWeek"].ToString()!) : null,
                    HalfDayClosingTime = reader["HalfDayClosingTime"] != DBNull.Value ? (TimeSpan?)reader["HalfDayClosingTime"] : null,
                    LateLimitMinutes = Convert.ToInt32(reader["LateLimitMinutes"]),
                    EarlyOutLimitMinutes = Convert.ToInt32(reader["EarlyOutLimitMinutes"]),
                    Weekends = reader["Weekends"]?.ToString() ?? "",
                    HalfDays = reader["HalfDays"]?.ToString() ?? ""
                };
            }
            return null;
        }

        private List<SelectListItem> GetSelectList(string query, string valueField, string textField, int? campusId = null)
        {
            var list = new List<SelectListItem>();
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(query, con);
            if (campusId.HasValue)
                cmd.Parameters.AddWithValue("@CampusId", campusId.Value);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader[valueField].ToString(),
                    Text = reader[textField].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetCampuses() => GetSelectList("SELECT CampusId, CampusName FROM Campuses", "CampusId", "CampusName");
        private List<SelectListItem> GetTimeTables() => GetSelectList("SELECT Id, Title FROM TimeTableManagement", "Id", "Title");
        private List<SelectListItem> GetBatches() => GetSelectList("SELECT BatchId, BatchName FROM Batches", "BatchId", "BatchName");
        private List<SelectListItem> GetEmployees() => GetSelectList("SELECT EmployeeId, EmployeeName FROM Employees", "EmployeeId", "EmployeeName");

        private List<dynamic> GetAppliedTimeTables()
        {
            var list = new List<dynamic>();
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"SELECT a.Id, c.CampusName, t.Title AS TimeTableTitle,
                                a.ApplyTo, a.SelectedIds, a.StartDate, a.EndDate
                                FROM ApplyTimeTable a
                                LEFT JOIN TimeTableManagement t ON a.TimeTableId = t.Id
                                LEFT JOIN Campuses c ON a.CampusId = c.CampusId
                                ORDER BY a.Id DESC", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CampusName = reader["CampusName"]?.ToString(),
                    TimeTableTitle = reader["TimeTableTitle"]?.ToString(),
                    ApplyTo = reader["ApplyTo"]?.ToString(),
                    SelectedIds = reader["SelectedIds"]?.ToString(),
                    StartDate = Convert.ToDateTime(reader["StartDate"]).ToShortDateString(),
                    EndDate = Convert.ToDateTime(reader["EndDate"]).ToShortDateString()
                });
            }
            return list;
        }

        private WeekendHalfDaySetup? GetWeekendHalfDaySetup(int campusId, DateTime date)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"SELECT TOP 1 * FROM WeekendHalfDaySetup 
                WHERE CampusId = @CampusId AND StartDate <= @Date AND EndDate >= @Date", con);
            cmd.Parameters.AddWithValue("@CampusId", campusId);
            cmd.Parameters.AddWithValue("@Date", date);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new WeekendHalfDaySetup
                {
                    WeekendDays = JsonSerializer.Deserialize<List<DayOfWeek>>(reader["WeekendDays"]?.ToString() ?? "[]") ?? new(),
                    HalfDayWeekdays = JsonSerializer.Deserialize<List<DayOfWeek>>(reader["HalfDayWeekdays"]?.ToString() ?? "[]") ?? new()
                };
            }
            return null;
        }
    }
}
