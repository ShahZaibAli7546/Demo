using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers;

public class EmployeeLeaveManagementController(IConfiguration configuration) : Controller
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    // ===================== INDEX =====================
    public IActionResult Index()
    {
        var viewModel = new EmployeeLeaveManagementViewModel
        {
            LeaveAllotments = new()
            {
                new() // default item for form binding
            },
            LeaveYears = new()
            {
                new() { StartDate = DateTime.Today, EndDate = DateTime.Today }
            }
        };

        using var con = new SqlConnection(connectionString);
        con.Open();

        // Load Leave Allotments
        using (var cmd = new SqlCommand("SELECT * FROM LeaveAllotment", con))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                viewModel.LeaveAllotments.Add(new()
                {
                    Id = reader.GetInt32(0),
                    CampusId = reader.GetInt32(1),
                    EmployeeId = reader.GetInt32(2),
                    LeaveTypeId = reader.GetInt32(3),
                    TotalDays = reader.GetInt32(4),
                    Remarks = reader["Remarks"]?.ToString()
                });
            }
        }

        // Load Leave Years
        using (var cmd = new SqlCommand("SELECT * FROM LeaveYear", con))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                viewModel.LeaveYears.Add(new()
                {
                    Id = reader.GetInt32(0),
                    Title = reader["Title"]?.ToString() ?? "",
                    Status = reader["Status"]?.ToString() ?? "Active",
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"])
                });
            }
        }

        // Static dropdowns
        ViewBag.Campuses = new List<SelectListItem>
        {
            new() { Value = "1", Text = "Main Campus" },
            new() { Value = "2", Text = "City Campus" }
        };

        ViewBag.Employees = new List<SelectListItem>
        {
            new() { Value = "1", Text = "Ali Raza" },
            new() { Value = "2", Text = "Fatima Khan" }
        };

        ViewBag.LeaveTypes = new List<SelectListItem>
        {
            new() { Value = "1", Text = "Sick Leave" },
            new() { Value = "2", Text = "Casual Leave" }
        };

        return View(viewModel);
    }

    // ===================== CREATE LEAVE ALLOTMENT =====================
    [HttpPost]
    public IActionResult CreateAllotment(LeaveAllotment model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid leave allotment data.";
            return RedirectToAction("Index");
        }

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(@"
            INSERT INTO LeaveAllotment (CampusId, EmployeeId, LeaveTypeId, TotalDays, Remarks)
            VALUES (@CampusId, @EmployeeId, @LeaveTypeId, @TotalDays, @Remarks)", con);

        cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
        cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
        cmd.Parameters.AddWithValue("@LeaveTypeId", model.LeaveTypeId);
        cmd.Parameters.AddWithValue("@TotalDays", model.TotalDays);
        cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "Leave allotment added.";
        return RedirectToAction("Index");
    }

    // ===================== CREATE LEAVE YEAR =====================
    [HttpPost]
    public IActionResult CreateLeaveYear(LeaveYear model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid leave year data.";
            return RedirectToAction("Index");
        }

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(@"
            INSERT INTO LeaveYear (Title, Status, StartDate, EndDate)
            VALUES (@Title, @Status, @StartDate, @EndDate)", con);

        cmd.Parameters.AddWithValue("@Title", model.Title);
        cmd.Parameters.AddWithValue("@Status", model.Status);
        cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", model.EndDate);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "Leave year added.";
        return RedirectToAction("Index");
    }
}
