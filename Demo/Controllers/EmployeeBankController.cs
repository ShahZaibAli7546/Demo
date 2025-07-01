using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Demo.Controllers
{
    public class EmployeeBankController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");

        // 🔷 INDEX
        public IActionResult Index()
        {
            var banks = new List<EmployeeBank>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM EmployeeBank", con);

            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                banks.Add(new()
                {
                    Id = reader.GetInt32(0),
                    EmployeeId = reader.GetInt32(1),
                    BankName = reader["BankName"].ToString() ?? "",
                    AccountNumber = reader["AccountNumber"].ToString() ?? "",
                    BranchCode = reader["BranchCode"]?.ToString()
                });
            }

            return View(banks);
        }

        // 🔹 CREATE (GET)
        public IActionResult Create() => View();

        // 🔹 CREATE (POST)
        [HttpPost]
        public IActionResult Create(EmployeeBank model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO EmployeeBank (EmployeeId, BankName, AccountNumber, BranchCode)
                VALUES (@EmployeeId, @BankName, @AccountNumber, @BranchCode)", con);

            cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
            cmd.Parameters.AddWithValue("@BankName", model.BankName);
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);
            cmd.Parameters.AddWithValue("@BranchCode", (object?)model.BranchCode ?? DBNull.Value);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Bank information added.";
            return RedirectToAction("Index");
        }

        // ✏️ EDIT (GET)
        public IActionResult Edit(int id)
        {
            EmployeeBank? bank = null;

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM EmployeeBank WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                bank = new()
                {
                    Id = reader.GetInt32(0),
                    EmployeeId = reader.GetInt32(1),
                    BankName = reader["BankName"].ToString() ?? "",
                    AccountNumber = reader["AccountNumber"].ToString() ?? "",
                    BranchCode = reader["BranchCode"]?.ToString()
                };
            }

            return bank == null ? NotFound() : View(bank);
        }

        // ✏️ EDIT (POST)
        [HttpPost]
        public IActionResult Edit(EmployeeBank model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE EmployeeBank SET 
                    EmployeeId = @EmployeeId,
                    BankName = @BankName,
                    AccountNumber = @AccountNumber,
                    BranchCode = @BranchCode
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
            cmd.Parameters.AddWithValue("@BankName", model.BankName);
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);
            cmd.Parameters.AddWithValue("@BranchCode", (object?)model.BranchCode ?? DBNull.Value);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Bank information updated.";
            return RedirectToAction("Index");
        }

        // ❌ DELETE (GET)
        public IActionResult Delete(int id)
        {
            EmployeeBank? bank = null;

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM EmployeeBank WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                bank = new()
                {
                    Id = reader.GetInt32(0),
                    EmployeeId = reader.GetInt32(1),
                    BankName = reader["BankName"].ToString() ?? "",
                    AccountNumber = reader["AccountNumber"].ToString() ?? "",
                    BranchCode = reader["BranchCode"]?.ToString()
                };
            }

            return bank == null ? NotFound() : View(bank);
        }

        // ❌ DELETE (POST)
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM EmployeeBank WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["Success"] = "Bank information deleted.";
            return RedirectToAction("Index");
        }
    }
}
