using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers;

public class FeeServiceController(IConfiguration configuration) : Controller
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found.");

    // ✅ Service Categories
    private static List<ServiceCategory> GetServiceCategories(string connectionString)
    {
        var list = new List<ServiceCategory>();
        using var con = new SqlConnection(connectionString);
        con.Open();
        using var cmd = new SqlCommand("SELECT CategoryId, CategoryName FROM ServiceCategories WHERE Status = 'Active'", con);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new()
            {
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                CategoryName = reader["CategoryName"]?.ToString() ?? ""
            });
        }
        return list;
    }

    // ✅ Payment Cycles
    private static List<PaymentCycle> GetPaymentCycles(string connectionString)
    {
        var list = new List<PaymentCycle>();
        using var con = new SqlConnection(connectionString);
        con.Open();
        using var cmd = new SqlCommand("SELECT Id, Name FROM PaymentCycle WHERE Status = 'Active'", con);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new()
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"]?.ToString() ?? ""
            });
        }
        return list;
    }

    // ✅ Account Dropdown (No ParentAccountId)
    private static List<AccountDropdownItem> GetAccounts(string connectionString)
    {
        var list = new List<AccountDropdownItem>();
        using var con = new SqlConnection(connectionString);
        con.Open();
        using var cmd = new SqlCommand("SELECT AccountId, AccountName FROM Accounts WHERE IsActive = 1", con);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new()
            {
                AccountId = Convert.ToInt32(reader["AccountId"]),
                DisplayName = reader["AccountName"]?.ToString() ?? ""
            });
        }
        return list;
    }

    // ✅ Index
    public IActionResult Index()
    {
        var list = new List<FeeService>();
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand(@"
            SELECT fs.*, sc.CategoryName, a.AccountName
            FROM FeeServices fs
            JOIN ServiceCategories sc ON fs.CategoryId = sc.CategoryId
            JOIN Accounts a ON fs.AccountId = a.AccountId", con);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new()
            {
                FeeServiceId = Convert.ToInt32(reader["FeeServiceId"]),
                FeeServiceName = reader["FeeServiceName"]?.ToString() ?? "",
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                AccountId = Convert.ToInt32(reader["AccountId"]),
                Cost = Convert.ToDecimal(reader["Cost"]),
                PaymentCycle = reader["PaymentCycle"]?.ToString() ?? "",
                Remarks = reader["Remarks"]?.ToString() ?? "",
                ShowOnVoucher = Convert.ToBoolean(reader["ShowOnVoucher"]),
                Taxable = Convert.ToBoolean(reader["Taxable"]),
                IsRoyalty = Convert.ToBoolean(reader["IsRoyalty"]),
                IsBehavioralFine = Convert.ToBoolean(reader["IsBehavioralFine"]),
                Status = reader["Status"]?.ToString() ?? "",
                CategoryName = reader["CategoryName"]?.ToString() ?? "",
                AccountName = reader["AccountName"]?.ToString() ?? ""
            });
        }

        return View(list);
    }

    // ✅ Create - GET
    public IActionResult Create()
    {
        ViewBag.Categories = GetServiceCategories(_connectionString);
        ViewBag.Accounts = GetAccounts(_connectionString);
        ViewBag.PaymentCycles = GetPaymentCycles(_connectionString);
        return View();
    }

    // ✅ Create - POST
    [HttpPost]
    public IActionResult Create(FeeService model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = GetServiceCategories(_connectionString);
            ViewBag.Accounts = GetAccounts(_connectionString);
            ViewBag.PaymentCycles = GetPaymentCycles(_connectionString);
            return View(model);
        }

        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand(@"
            INSERT INTO FeeServices
            (FeeServiceName, CategoryId, AccountId, Cost, PaymentCycle, Remarks, ShowOnVoucher, Taxable, IsRoyalty, IsBehavioralFine, Status)
            VALUES
            (@FeeServiceName, @CategoryId, @AccountId, @Cost, @PaymentCycle, @Remarks, @ShowOnVoucher, @Taxable, @IsRoyalty, @IsBehavioralFine, @Status)", con);

        cmd.Parameters.AddWithValue("@FeeServiceName", model.FeeServiceName);
        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
        cmd.Parameters.AddWithValue("@AccountId", model.AccountId);
        cmd.Parameters.AddWithValue("@Cost", model.Cost);
        cmd.Parameters.AddWithValue("@PaymentCycle", model.PaymentCycle ?? "");
        cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
        cmd.Parameters.AddWithValue("@ShowOnVoucher", model.ShowOnVoucher);
        cmd.Parameters.AddWithValue("@Taxable", model.Taxable);
        cmd.Parameters.AddWithValue("@IsRoyalty", model.IsRoyalty);
        cmd.Parameters.AddWithValue("@IsBehavioralFine", model.IsBehavioralFine);
        cmd.Parameters.AddWithValue("@Status", model.Status ?? "Active");

        cmd.ExecuteNonQuery();
        TempData["SuccessMessage"] = "✅ Fee Service created successfully.";
        return RedirectToAction("Index");
    }

    // ✅ Edit - GET
    public IActionResult Edit(int id)
    {
        FeeService model = new();
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand("SELECT * FROM FeeServices WHERE FeeServiceId = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new()
            {
                FeeServiceId = id,
                FeeServiceName = reader["FeeServiceName"]?.ToString() ?? "",
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                AccountId = Convert.ToInt32(reader["AccountId"]),
                Cost = Convert.ToDecimal(reader["Cost"]),
                PaymentCycle = reader["PaymentCycle"]?.ToString() ?? "",
                Remarks = reader["Remarks"]?.ToString() ?? "",
                ShowOnVoucher = Convert.ToBoolean(reader["ShowOnVoucher"]),
                Taxable = Convert.ToBoolean(reader["Taxable"]),
                IsRoyalty = Convert.ToBoolean(reader["IsRoyalty"]),
                IsBehavioralFine = Convert.ToBoolean(reader["IsBehavioralFine"]),
                Status = reader["Status"]?.ToString() ?? ""
            };
        }

        ViewBag.Categories = GetServiceCategories(_connectionString);
        ViewBag.Accounts = GetAccounts(_connectionString);
        ViewBag.PaymentCycles = GetPaymentCycles(_connectionString);
        return View(model);
    }

    // ✅ Edit - POST
    [HttpPost]
    public IActionResult Edit(FeeService model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = GetServiceCategories(_connectionString);
            ViewBag.Accounts = GetAccounts(_connectionString);
            ViewBag.PaymentCycles = GetPaymentCycles(_connectionString);
            return View(model);
        }

        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand(@"
            UPDATE FeeServices SET
                FeeServiceName = @FeeServiceName,
                CategoryId = @CategoryId,
                AccountId = @AccountId,
                Cost = @Cost,
                PaymentCycle = @PaymentCycle,
                Remarks = @Remarks,
                ShowOnVoucher = @ShowOnVoucher,
                Taxable = @Taxable,
                IsRoyalty = @IsRoyalty,
                IsBehavioralFine = @IsBehavioralFine,
                Status = @Status
            WHERE FeeServiceId = @Id", con);

        cmd.Parameters.AddWithValue("@Id", model.FeeServiceId);
        cmd.Parameters.AddWithValue("@FeeServiceName", model.FeeServiceName);
        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
        cmd.Parameters.AddWithValue("@AccountId", model.AccountId);
        cmd.Parameters.AddWithValue("@Cost", model.Cost);
        cmd.Parameters.AddWithValue("@PaymentCycle", model.PaymentCycle ?? "");
        cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
        cmd.Parameters.AddWithValue("@ShowOnVoucher", model.ShowOnVoucher);
        cmd.Parameters.AddWithValue("@Taxable", model.Taxable);
        cmd.Parameters.AddWithValue("@IsRoyalty", model.IsRoyalty);
        cmd.Parameters.AddWithValue("@IsBehavioralFine", model.IsBehavioralFine);
        cmd.Parameters.AddWithValue("@Status", model.Status ?? "Active");

        cmd.ExecuteNonQuery();
        TempData["SuccessMessage"] = "✅ Fee Service updated successfully.";
        return RedirectToAction("Index");
    }

    // ✅ Delete - GET
    public IActionResult Delete(int id)
    {
        FeeService model = new();
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand("SELECT FeeServiceId, FeeServiceName FROM FeeServices WHERE FeeServiceId = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            model = new()
            {
                FeeServiceId = id,
                FeeServiceName = reader["FeeServiceName"]?.ToString() ?? ""
            };
        }

        return View(model);
    }

    // ✅ Delete - POST
    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var cmd = new SqlCommand("DELETE FROM FeeServices WHERE FeeServiceId = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
        TempData["SuccessMessage"] = "✅ Fee Service deleted successfully.";
        return RedirectToAction("Index");
    }

    // ✅ Helper Class
    public class AccountDropdownItem
    {
        public int AccountId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
