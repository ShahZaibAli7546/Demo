using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class PayslipContributionItemsController : Controller
    {
        private readonly IConfiguration _configuration;
        public PayslipContributionItemsController(IConfiguration configuration)
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
        public IActionResult Create(PayslipContributionItem model)
        {
            if (!ModelState.IsValid)
            {
                LoadAccounts();
                return View(model);
            }

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(@"INSERT INTO PayslipContributionItems 
                    (Title, ExpenseAccountId, LiabilityAccountId) 
                    VALUES (@Title, @ExpenseAccountId, @LiabilityAccountId)", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@ExpenseAccountId", model.ExpenseAccountId);
            cmd.Parameters.AddWithValue("@LiabilityAccountId", model.LiabilityAccountId);

            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index", "PayslipItems");
        }

        public IActionResult Edit(int id)
        {
            PayslipContributionItem model = new();

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT * FROM PayslipContributionItems WHERE Id = @Id", con);
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

        [HttpPost]
        public IActionResult Edit(PayslipContributionItem model)
        {
            if (!ModelState.IsValid)
            {
                LoadAccounts();
                return View(model);
            }

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(@"UPDATE PayslipContributionItems SET 
                    Title = @Title, ExpenseAccountId = @ExpenseAccountId, 
                    LiabilityAccountId = @LiabilityAccountId 
                    WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@ExpenseAccountId", model.ExpenseAccountId);
            cmd.Parameters.AddWithValue("@LiabilityAccountId", model.LiabilityAccountId);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index", "PayslipItems");
        }

        public IActionResult Delete(int id)
        {
            PayslipContributionItem model = new();

            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new("SELECT * FROM PayslipContributionItems WHERE Id = @Id", con);
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
            using SqlCommand cmd = new("DELETE FROM PayslipContributionItems WHERE Id = @Id", con);
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
