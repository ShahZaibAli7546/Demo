using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class PayslipDeductionItemsController : Controller
    {
        private readonly IConfiguration _configuration;
        public PayslipDeductionItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Create()
        {
            LoadAccounts();
            return View();
        }

        [HttpPost]
        public IActionResult Create(PayslipDeductionItem model)
        {
            if (!ModelState.IsValid)
            {
                LoadAccounts();
                return View(model);
            }

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(@"INSERT INTO PayslipDeductionItems 
                    (Title, ExpenseAccountId, LiabilityAccountId, IsAttendance, IsPayable) 
                    VALUES (@Title, @ExpenseAccountId, @LiabilityAccountId, @IsAttendance, @IsPayable)", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@ExpenseAccountId", model.ExpenseAccountId);
            cmd.Parameters.AddWithValue("@LiabilityAccountId", model.LiabilityAccountId);
            cmd.Parameters.AddWithValue("@IsAttendance", model.IsAttendance);
            cmd.Parameters.AddWithValue("@IsPayable", model.IsPayable);

            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index", "PayslipItems");
        }

        public IActionResult Edit(int id)
        {
            PayslipDeductionItem model = new();

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT * FROM PayslipDeductionItems WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Title = reader["Title"].ToString()!;
                model.ExpenseAccountId = Convert.ToInt32(reader["ExpenseAccountId"]);
                model.LiabilityAccountId = Convert.ToInt32(reader["LiabilityAccountId"]);
                model.IsAttendance = Convert.ToBoolean(reader["IsAttendance"]);
                model.IsPayable = Convert.ToBoolean(reader["IsPayable"]);
            }

            LoadAccounts();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(PayslipDeductionItem model)
        {
            if (!ModelState.IsValid)
            {
                LoadAccounts();
                return View(model);
            }

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(@"UPDATE PayslipDeductionItems SET 
                    Title = @Title, ExpenseAccountId = @ExpenseAccountId, 
                    LiabilityAccountId = @LiabilityAccountId, 
                    IsAttendance = @IsAttendance, IsPayable = @IsPayable 
                    WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@ExpenseAccountId", model.ExpenseAccountId);
            cmd.Parameters.AddWithValue("@LiabilityAccountId", model.LiabilityAccountId);
            cmd.Parameters.AddWithValue("@IsAttendance", model.IsAttendance);
            cmd.Parameters.AddWithValue("@IsPayable", model.IsPayable);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index", "PayslipItems");
        }

        public IActionResult Delete(int id)
        {
            PayslipDeductionItem model = new();

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT * FROM PayslipDeductionItems WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Title = reader["Title"].ToString()!;
                model.ExpenseAccountId = Convert.ToInt32(reader["ExpenseAccountId"]);
                model.LiabilityAccountId = Convert.ToInt32(reader["LiabilityAccountId"]);
            }

            LoadAccounts();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("DELETE FROM PayslipDeductionItems WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index", "PayslipItems");
        }

        private void LoadAccounts()
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

            ViewBag.Accounts = accounts;
        }
    }
}
