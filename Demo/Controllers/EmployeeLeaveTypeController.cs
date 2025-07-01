using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Demo.Models;

namespace Demo.Controllers;

public class EmployeeLeaveTypeController(IConfiguration configuration) : Controller
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public IActionResult Index()
    {
        var leaveTypes = new List<EmployeeLeaveType>();

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT * FROM EmployeeLeaveType", con);
        con.Open();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            leaveTypes.Add(new EmployeeLeaveType
            {
                Id = reader.GetInt32(0),
                Name = reader["Name"].ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                BackgroundColor = reader["BackgroundColor"].ToString() ?? "#ffffff",
                Symbol = reader["Symbol"].ToString() ?? "🗓️",
                Weightage = reader["Weightage"].ToString() ?? "Full",
                Status = reader["Status"].ToString() ?? "Active"
            });
        }

        return View(leaveTypes);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(EmployeeLeaveType model)
    {
        if (!ModelState.IsValid) return View(model);

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(@"
            INSERT INTO EmployeeLeaveType (Name, Description, BackgroundColor, Symbol, Weightage, Status)
            VALUES (@Name, @Description, @BackgroundColor, @Symbol, @Weightage, @Status)", con);

        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BackgroundColor", model.BackgroundColor);
        cmd.Parameters.AddWithValue("@Symbol", model.Symbol);
        cmd.Parameters.AddWithValue("@Weightage", model.Weightage);
        cmd.Parameters.AddWithValue("@Status", model.Status);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Leave type created successfully.";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var model = new EmployeeLeaveType();

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("SELECT * FROM EmployeeLeaveType WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model.Id = reader.GetInt32(0);
            model.Name = reader["Name"].ToString() ?? string.Empty;
            model.Description = reader["Description"]?.ToString();
            model.BackgroundColor = reader["BackgroundColor"].ToString() ?? "#ffffff";
            model.Symbol = reader["Symbol"].ToString() ?? "🗓️";
            model.Weightage = reader["Weightage"].ToString() ?? "Full";
            model.Status = reader["Status"].ToString() ?? "Active";
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(EmployeeLeaveType model)
    {
        if (!ModelState.IsValid) return View(model);

        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(@"
            UPDATE EmployeeLeaveType SET
                Name = @Name,
                Description = @Description,
                BackgroundColor = @BackgroundColor,
                Symbol = @Symbol,
                Weightage = @Weightage,
                Status = @Status
            WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Id", model.Id);
        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BackgroundColor", model.BackgroundColor);
        cmd.Parameters.AddWithValue("@Symbol", model.Symbol);
        cmd.Parameters.AddWithValue("@Weightage", model.Weightage);
        cmd.Parameters.AddWithValue("@Status", model.Status);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Leave type updated successfully.";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand("DELETE FROM EmployeeLeaveType WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Leave type deleted successfully.";
        return RedirectToAction("Index");
    }
}
