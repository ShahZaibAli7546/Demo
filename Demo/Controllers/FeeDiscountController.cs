using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class FeeDiscountController : Controller
    {
        private readonly IConfiguration _configuration;

        public FeeDiscountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        private List<SelectListItem> GetDiscountCategoryList()
        {
            var list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT DiscountCategoryId, DiscountCategoryName FROM DiscountCategory WHERE Status = 'Active'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["DiscountCategoryId"].ToString() ?? "",
                        Text = reader["DiscountCategoryName"].ToString() ?? ""
                    });
                }
            }

            return list;
        }

        private List<SelectListItem> GetDiscountTypeList() => new()
        {
            new SelectListItem { Text = "Percentage", Value = "Percentage" },
            new SelectListItem { Text = "Fixed Amount", Value = "Fixed Amount" }
        };

        private List<SelectListItem> GetStatusList() => new()
        {
            new SelectListItem { Text = "Active", Value = "Active" },
            new SelectListItem { Text = "Inactive", Value = "Inactive" }
        };

        public IActionResult Index()
        {
            var list = new List<FeeDiscount>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = @"
                    SELECT fd.*, dc.DiscountCategoryName 
                    FROM FeeDiscount fd 
                    JOIN DiscountCategory dc ON fd.DiscountCategoryId = dc.DiscountCategoryId";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new FeeDiscount
                    {
                        FeeDiscountId = Convert.ToInt32(reader["FeeDiscountId"]),
                        FeeDiscountName = reader["FeeDiscountName"].ToString() ?? string.Empty,
                        DiscountCategoryId = Convert.ToInt32(reader["DiscountCategoryId"]),
                        DiscountCategoryName = reader["DiscountCategoryName"].ToString() ?? string.Empty,
                        DiscountType = reader["DiscountType"].ToString() ?? string.Empty,
                        Percentage = reader["Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["Percentage"]) : null,
                        Amount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : null,
                        Remarks = reader["Remarks"]?.ToString(),
                        Status = reader["Status"].ToString() ?? string.Empty
                    });
                }
            }

            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.DiscountCategoryList = GetDiscountCategoryList();
            ViewBag.DiscountTypeList = GetDiscountTypeList();
            ViewBag.StatusList = GetStatusList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(FeeDiscount model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    string query = @"
                        INSERT INTO FeeDiscount 
                        (FeeDiscountName, DiscountCategoryId, DiscountType, Percentage, Amount, Remarks, Status) 
                        VALUES 
                        (@FeeDiscountName, @DiscountCategoryId, @DiscountType, @Percentage, @Amount, @Remarks, @Status)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FeeDiscountName", model.FeeDiscountName);
                    cmd.Parameters.AddWithValue("@DiscountCategoryId", model.DiscountCategoryId);
                    cmd.Parameters.AddWithValue("@DiscountType", model.DiscountType);
                    cmd.Parameters.AddWithValue("@Percentage", (object?)model.Percentage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", (object?)model.Amount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Discount created successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.DiscountCategoryList = GetDiscountCategoryList();
            ViewBag.DiscountTypeList = GetDiscountTypeList();
            ViewBag.StatusList = GetStatusList();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            FeeDiscount model = new FeeDiscount();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM FeeDiscount WHERE FeeDiscountId = @FeeDiscountId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FeeDiscountId", id);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.FeeDiscountId = Convert.ToInt32(reader["FeeDiscountId"]);
                    model.FeeDiscountName = reader["FeeDiscountName"].ToString() ?? string.Empty;
                    model.DiscountCategoryId = Convert.ToInt32(reader["DiscountCategoryId"]);
                    model.DiscountType = reader["DiscountType"].ToString() ?? string.Empty;
                    model.Percentage = reader["Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["Percentage"]) : null;
                    model.Amount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : null;
                    model.Remarks = reader["Remarks"]?.ToString();
                    model.Status = reader["Status"].ToString() ?? string.Empty;
                }
            }

            ViewBag.DiscountCategoryList = GetDiscountCategoryList();
            ViewBag.DiscountTypeList = GetDiscountTypeList();
            ViewBag.StatusList = GetStatusList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(FeeDiscount model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    string query = @"
                        UPDATE FeeDiscount 
                        SET FeeDiscountName = @FeeDiscountName, 
                            DiscountCategoryId = @DiscountCategoryId, 
                            DiscountType = @DiscountType, 
                            Percentage = @Percentage, 
                            Amount = @Amount, 
                            Remarks = @Remarks, 
                            Status = @Status 
                        WHERE FeeDiscountId = @FeeDiscountId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FeeDiscountId", model.FeeDiscountId);
                    cmd.Parameters.AddWithValue("@FeeDiscountName", model.FeeDiscountName);
                    cmd.Parameters.AddWithValue("@DiscountCategoryId", model.DiscountCategoryId);
                    cmd.Parameters.AddWithValue("@DiscountType", model.DiscountType);
                    cmd.Parameters.AddWithValue("@Percentage", (object?)model.Percentage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", (object?)model.Amount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Discount updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.DiscountCategoryList = GetDiscountCategoryList();
            ViewBag.DiscountTypeList = GetDiscountTypeList();
            ViewBag.StatusList = GetStatusList();
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string query = "DELETE FROM FeeDiscount WHERE FeeDiscountId = @FeeDiscountId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FeeDiscountId", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Discount deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
