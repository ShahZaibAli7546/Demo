using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers;

public class FeeStructureController(IConfiguration configuration) : Controller
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    // 🔹 INDEX
    public IActionResult Index()
    {
        var list = new List<FeeStructure>();

        using var con = new SqlConnection(connectionString);
        var query = @"
            SELECT fs.Id, fs.FeeServiceId, fs.FeeDiscountId, fs.Amount, fs.Remarks, fs.Status, fs.StructureName,
                   s.FeeServiceName, d.FeeDiscountName
            FROM FeeStructure fs
            LEFT JOIN FeeServices s ON fs.FeeServiceId = s.FeeServiceId
            LEFT JOIN FeeDiscount d ON fs.FeeDiscountId = d.FeeDiscountId";

        using var cmd = new SqlCommand(query, con);
        con.Open();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new FeeStructure
            {
                Id = reader.GetInt32(0),
                FeeServiceId = reader.GetInt32(1),
                FeeDiscountId = reader.GetInt32(2),
                Amount = Convert.ToDecimal(reader["Amount"]),
                Remarks = reader["Remarks"]?.ToString(),
                Status = reader["Status"]?.ToString() ?? "Active",
                StructureName = reader["StructureName"]?.ToString(),
                FeeServiceName = reader["FeeServiceName"]?.ToString(),
                FeeDiscountName = reader["FeeDiscountName"]?.ToString()
            });
        }

        return View(list);
    }

    // 🔹 CREATE
    public IActionResult Create()
    {
        LoadDropdowns();
        return View(new FeeStructure());
    }

    [HttpPost]
    public IActionResult Create(FeeStructure model)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(model);
        }

        using var con = new SqlConnection(connectionString);
        var query = @"
            INSERT INTO FeeStructure (FeeServiceId, FeeDiscountId, Amount, Remarks, Status, StructureName)
            VALUES (@ServiceId, @DiscountId, @Amount, @Remarks, @Status, @StructureName)";

        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@ServiceId", model.FeeServiceId);
        cmd.Parameters.AddWithValue("@DiscountId", model.FeeDiscountId);
        cmd.Parameters.AddWithValue("@Amount", model.Amount);
        cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", model.Status ?? "Active");
        cmd.Parameters.AddWithValue("@StructureName", (object?)model.StructureName ?? DBNull.Value);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "Fee structure created successfully.";
        return RedirectToAction("Index");
    }

    // 🔹 EDIT
    public IActionResult Edit(int id)
    {
        FeeStructure? model = null;

        using var con = new SqlConnection(connectionString);
        var query = "SELECT * FROM FeeStructure WHERE Id = @Id";
        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new FeeStructure
            {
                Id = reader.GetInt32(0),
                FeeServiceId = reader.GetInt32(1),
                FeeDiscountId = reader.GetInt32(2),
                Amount = Convert.ToDecimal(reader["Amount"]),
                Remarks = reader["Remarks"]?.ToString(),
                Status = reader["Status"]?.ToString() ?? "Active",
                StructureName = reader["StructureName"]?.ToString()
            };
        }

        if (model == null)
            return NotFound();

        LoadDropdowns();
        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(FeeStructure model)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(model);
        }

        using var con = new SqlConnection(connectionString);
        var query = @"
            UPDATE FeeStructure SET
                FeeServiceId = @ServiceId,
                FeeDiscountId = @DiscountId,
                Amount = @Amount,
                Remarks = @Remarks,
                Status = @Status,
                StructureName = @StructureName
            WHERE Id = @Id";

        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Id", model.Id);
        cmd.Parameters.AddWithValue("@ServiceId", model.FeeServiceId);
        cmd.Parameters.AddWithValue("@DiscountId", model.FeeDiscountId);
        cmd.Parameters.AddWithValue("@Amount", model.Amount);
        cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", model.Status ?? "Active");
        cmd.Parameters.AddWithValue("@StructureName", (object?)model.StructureName ?? DBNull.Value);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "Fee structure updated successfully.";
        return RedirectToAction("Index");
    }

    // 🔹 DELETE
    public IActionResult Delete(int id)
    {
        using var con = new SqlConnection(connectionString);
        var query = "DELETE FROM FeeStructure WHERE Id = @Id";
        using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "Fee structure deleted.";
        return RedirectToAction("Index");
    }

    // 🔁 Dropdowns from DB
    private void LoadDropdowns()
    {
        var services = new List<SelectListItem>();
        var discounts = new List<SelectListItem>();

        using var con = new SqlConnection(connectionString);
        con.Open();

        // 🔷 FeeServices
        using (var cmd = new SqlCommand("SELECT FeeServiceId, FeeServiceName FROM FeeServices", con))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                services.Add(new SelectListItem
                {
                    Value = reader["FeeServiceId"].ToString(),
                    Text = reader["FeeServiceName"].ToString()
                });
            }
        }

        // 🔷 FeeDiscount
        using (var cmd = new SqlCommand("SELECT FeeDiscountId, FeeDiscountName FROM FeeDiscount", con))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                discounts.Add(new SelectListItem
                {
                    Value = reader["FeeDiscountId"].ToString(),
                    Text = reader["FeeDiscountName"].ToString()
                });
            }
        }

        ViewBag.Services = services;
        ViewBag.Discounts = discounts;
    }
}
