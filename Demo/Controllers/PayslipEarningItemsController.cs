using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class PayslipEarningItemsController : Controller
    {
        private readonly IConfiguration _configuration;

        public PayslipEarningItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection")!;

        // 🔷 INDEX
        public IActionResult Index()
        {
            List<PayslipEarningItem> items = [];

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT e.Id, e.Title, e.ExpenseAccountId, a.Title AS ExpenseAccountName FROM PayslipEarningItems e INNER JOIN Accounts a ON e.ExpenseAccountId = a.Id", con);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new PayslipEarningItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Title = reader["Title"].ToString() ?? "",
                    ExpenseAccountId = Convert.ToInt32(reader["ExpenseAccountId"]),
                    ExpenseAccountName = reader["ExpenseAccountName"].ToString() ?? ""
                });
            }
            return View(items);
        }

        // 🔹 GET: Create
        public IActionResult Create()
        {
            ViewBag.Accounts = GetAccounts();
            return View();
        }

        // 🔹 POST: Create
        [HttpPost]
        public IActionResult Create(PayslipEarningItem model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Accounts = GetAccounts();
                return View(model);
            }

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("INSERT INTO PayslipEarningItems (Title, ExpenseAccountId) VALUES (@Title, @ExpenseAccountId)", con);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@ExpenseAccountId", model.ExpenseAccountId);

            con.Open();
            cmd.ExecuteNonQuery();

            // ✅ Redirect to PayslipItems/Index
            return RedirectToAction("Index", "PayslipItems");
        }

        // 🔹 GET: Edit
        public IActionResult Edit(int id)
        {
            PayslipEarningItem model = new();

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT Id, Title, ExpenseAccountId FROM PayslipEarningItems WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Title = reader["Title"].ToString() ?? "";
                model.ExpenseAccountId = Convert.ToInt32(reader["ExpenseAccountId"]);
            }

            ViewBag.Accounts = GetAccounts();
            return View(model);
        }

        // 🔹 POST: Edit
        [HttpPost]
        public IActionResult Edit(PayslipEarningItem model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Accounts = GetAccounts();
                return View(model);
            }

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("UPDATE PayslipEarningItems SET Title = @Title, ExpenseAccountId = @ExpenseAccountId WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@ExpenseAccountId", model.ExpenseAccountId);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            con.Open();
            cmd.ExecuteNonQuery();

            // ✅ Redirect to PayslipItems/Index
            return RedirectToAction("Index", "PayslipItems");
        }

        // 🔹 GET: Delete
        public IActionResult Delete(int id)
        {
            PayslipEarningItem model = new();

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT Id, Title, ExpenseAccountId FROM PayslipEarningItems WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Title = reader["Title"].ToString() ?? "";
                model.ExpenseAccountId = Convert.ToInt32(reader["ExpenseAccountId"]);
            }

            ViewBag.Accounts = GetAccounts();
            return View(model);
        }

        // 🔹 POST: Delete Confirmed
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("DELETE FROM PayslipEarningItems WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();

            // ✅ Redirect to PayslipItems/Index
            return RedirectToAction("Index", "PayslipItems");
        }

        // 🔸 Private Method to Fetch Active Accounts
        private List<SelectListItem> GetAccounts()
        {
            List<SelectListItem> accounts = [];

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT Id, Title FROM Accounts WHERE IsActive = 1 ORDER BY Title", con);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                accounts.Add(new SelectListItem
                {
                    Value = reader["Id"].ToString(),
                    Text = reader["Title"].ToString()
                });
            }

            return accounts;
        }
    }
}
