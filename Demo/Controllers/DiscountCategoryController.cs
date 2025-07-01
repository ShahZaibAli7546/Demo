using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers;

public class DiscountCategoryController(IConfiguration configuration) : Controller
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

    public IActionResult Index()
    {
        var list = new List<DiscountCategory>();

        using var con = new SqlConnection(_connectionString);
        const string query = "SELECT * FROM DiscountCategory";
        using var cmd = new SqlCommand(query, con);

        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new DiscountCategory
            {
                Id = Convert.ToInt32(reader["Id"]),
                Title = reader["Title"].ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                Status = reader["Status"].ToString() ?? string.Empty
            });
        }

        return View(list);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(DiscountCategory model)
    {
        if (!ModelState.IsValid) return View(model);

        using var con = new SqlConnection(_connectionString);
        const string query = @"INSERT INTO DiscountCategory (Title, Description, Status)
                               VALUES (@Title, @Description, @Status)";
        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Title", model.Title);
        cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", model.Status);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Category created successfully.";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var model = new DiscountCategory();

        using var con = new SqlConnection(_connectionString);
        const string query = "SELECT * FROM DiscountCategory WHERE Id = @Id";
        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model.Id = Convert.ToInt32(reader["Id"]);
            model.Title = reader["Title"].ToString() ?? string.Empty;
            model.Description = reader["Description"]?.ToString();
            model.Status = reader["Status"].ToString() ?? string.Empty;
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(DiscountCategory model)
    {
        if (!ModelState.IsValid) return View(model);

        using var con = new SqlConnection(_connectionString);
        const string query = @"UPDATE DiscountCategory
                               SET Title = @Title, Description = @Description, Status = @Status
                               WHERE Id = @Id";
        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Id", model.Id);
        cmd.Parameters.AddWithValue("@Title", model.Title);
        cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", model.Status);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Category updated successfully.";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        using var con = new SqlConnection(_connectionString);
        const string query = "DELETE FROM DiscountCategory WHERE Id = @Id";
        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Category deleted successfully.";
        return RedirectToAction("Index");
    }
}
