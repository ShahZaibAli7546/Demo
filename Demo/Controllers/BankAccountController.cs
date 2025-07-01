using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers;

public class BankAccountController(IConfiguration configuration) : Controller
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found.");

    // 🔷 INDEX
    public IActionResult Index()
    {
        var list = new List<BankAccount>();

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM BankAccounts", con);
        con.Open();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new BankAccount
            {
                Id = Convert.ToInt32(reader["Id"]),
                BankAccountName = reader["BankAccountName"].ToString() ?? "",
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
    public IActionResult Create(BankAccount model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand(@"
            INSERT INTO BankAccounts (BankAccountName, OpeningBalance, TransferFullAmount)
            VALUES (@BankAccountName, @OpeningBalance, @TransferFullAmount)", con);

        cmd.Parameters.AddWithValue("@BankAccountName", model.BankAccountName);
        cmd.Parameters.AddWithValue("@OpeningBalance", model.OpeningBalance);
        cmd.Parameters.AddWithValue("@TransferFullAmount", model.TransferFullAmount);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "✅ Bank account added.";
        return RedirectToAction("Index");
    }

    // ✏️ EDIT (GET)
    public IActionResult Edit(int id)
    {
        BankAccount? model = null;

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM BankAccounts WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new BankAccount
            {
                Id = Convert.ToInt32(reader["Id"]),
                BankAccountName = reader["BankAccountName"].ToString() ?? "",
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
    public IActionResult Edit(BankAccount model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand(@"
            UPDATE BankAccounts SET 
                BankAccountName = @BankAccountName,
                OpeningBalance = @OpeningBalance,
                TransferFullAmount = @TransferFullAmount
            WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Id", model.Id);
        cmd.Parameters.AddWithValue("@BankAccountName", model.BankAccountName);
        cmd.Parameters.AddWithValue("@OpeningBalance", model.OpeningBalance);
        cmd.Parameters.AddWithValue("@TransferFullAmount", model.TransferFullAmount);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "✅ Bank account updated.";
        return RedirectToAction("Index");
    }

    // ❌ DELETE (GET)
    public IActionResult Delete(int id)
    {
        BankAccount? model = null;

        using var con = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM BankAccounts WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new BankAccount
            {
                Id = Convert.ToInt32(reader["Id"]),
                BankAccountName = reader["BankAccountName"].ToString() ?? ""
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
        var cmd = new SqlCommand("DELETE FROM BankAccounts WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        cmd.ExecuteNonQuery();

        TempData["Success"] = "✅ Bank account deleted.";
        return RedirectToAction("Index");
    }
}
