using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers;

public class CashAccountController(IConfiguration configuration) : Controller
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found.");

    // 🔷 INDEX
    public IActionResult Index()
    {
        var list = new List<CashAccount>();

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM CashAccounts", con);
        con.Open();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new CashAccount
            {
                Id = Convert.ToInt32(reader["Id"]),
                CashAccountName = reader["CashAccountName"].ToString() ?? "",
                OpeningBalance = Convert.ToDecimal(reader["OpeningBalance"]),
                TransferFullAmount = Convert.ToBoolean(reader["TransferFullAmount"])
            });
        }

        ViewBag.TotalBalance = list.Sum(x => x.OpeningBalance);
        return View(list);
    }

    // 🔹 CREATE (GET)
    public IActionResult Create()
    {
        return View();
    }

    // 🔹 CREATE (POST)
    [HttpPost]
    public IActionResult Create(CashAccount model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand(@"
            INSERT INTO CashAccounts (CashAccountName, OpeningBalance, TransferFullAmount)
            VALUES (@CashAccountName, @OpeningBalance, @TransferFullAmount)", con);

        cmd.Parameters.AddWithValue("@CashAccountName", model.CashAccountName);
        cmd.Parameters.AddWithValue("@OpeningBalance", model.OpeningBalance);
        cmd.Parameters.AddWithValue("@TransferFullAmount", model.TransferFullAmount);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "✅ Cash account added.";
        return RedirectToAction("Index");
    }

    // ✏️ EDIT (GET)
    public IActionResult Edit(int id)
    {
        CashAccount? model = null;

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM CashAccounts WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new CashAccount
            {
                Id = Convert.ToInt32(reader["Id"]),
                CashAccountName = reader["CashAccountName"].ToString() ?? "",
                OpeningBalance = Convert.ToDecimal(reader["OpeningBalance"]),
                TransferFullAmount = Convert.ToBoolean(reader["TransferFullAmount"])
            };
        }

        if (model == null)
            return NotFound();

        return View(model);
    }

    // ✏️ EDIT (POST)
    [HttpPost]
    public IActionResult Edit(CashAccount model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand(@"
            UPDATE CashAccounts SET 
                CashAccountName = @CashAccountName,
                OpeningBalance = @OpeningBalance,
                TransferFullAmount = @TransferFullAmount
            WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Id", model.Id);
        cmd.Parameters.AddWithValue("@CashAccountName", model.CashAccountName);
        cmd.Parameters.AddWithValue("@OpeningBalance", model.OpeningBalance);
        cmd.Parameters.AddWithValue("@TransferFullAmount", model.TransferFullAmount);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "✅ Cash account updated.";
        return RedirectToAction("Index");
    }

    // ❌ DELETE (GET)
    public IActionResult Delete(int id)
    {
        CashAccount? model = null;

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM CashAccounts WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new CashAccount
            {
                Id = Convert.ToInt32(reader["Id"]),
                CashAccountName = reader["CashAccountName"].ToString() ?? ""
            };
        }

        if (model == null)
            return NotFound();

        return View(model);
    }

    // ❌ DELETE (POST)
    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("DELETE FROM CashAccounts WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "✅ Cash account deleted.";
        return RedirectToAction("Index");
    }
}
