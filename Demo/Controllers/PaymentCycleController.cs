using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers;

public class PaymentCycleController(IConfiguration configuration) : Controller
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

    public IActionResult Index()
    {
        List<PaymentCycle> list = [];

        using SqlConnection con = new(_connectionString);
        SqlCommand cmd = new("SELECT * FROM PaymentCycle ORDER BY Id DESC", con);
        con.Open();

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new PaymentCycle
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"]?.ToString() ?? "",
                Value = Convert.ToInt32(reader["Value"]),
                Status = reader["Status"]?.ToString() ?? ""
            });
        }

        return View(list);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(PaymentCycle model)
    {
        if (!ModelState.IsValid) return View(model);

        using SqlConnection con = new(_connectionString);
        SqlCommand cmd = new("INSERT INTO PaymentCycle (Name, Value, Status) VALUES (@Name, @Value, @Status)", con);
        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@Value", model.Value);
        cmd.Parameters.AddWithValue("@Status", model.Status);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Payment Cycle created successfully.";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        PaymentCycle? model = null;

        using SqlConnection con = new(_connectionString);
        SqlCommand cmd = new("SELECT * FROM PaymentCycle WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new PaymentCycle
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"]?.ToString() ?? "",
                Value = Convert.ToInt32(reader["Value"]),
                Status = reader["Status"]?.ToString() ?? ""
            };
        }

        return model == null ? NotFound() : View(model);
    }

    [HttpPost]
    public IActionResult Edit(PaymentCycle model)
    {
        if (!ModelState.IsValid) return View(model);

        using SqlConnection con = new(_connectionString);
        SqlCommand cmd = new("UPDATE PaymentCycle SET Name = @Name, Value = @Value, Status = @Status WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", model.Id);
        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@Value", model.Value);
        cmd.Parameters.AddWithValue("@Status", model.Status);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Payment Cycle updated successfully.";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        using SqlConnection con = new(_connectionString);
        SqlCommand cmd = new("DELETE FROM PaymentCycle WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["SuccessMessage"] = "Payment Cycle deleted successfully.";
        return RedirectToAction("Index");
    }
}
