using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class ServiceCategoryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ServiceCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                                  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }


        // GET: ServiceCategory
        public IActionResult Index()
        {
            List<ServiceCategory> categories = new List<ServiceCategory>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM ServiceCategories";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new ServiceCategory
                    {
                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                        CategoryName = reader["CategoryName"].ToString() ?? "",
                        Remarks = reader["Remarks"].ToString(),
                        Status = reader["Status"].ToString() ?? ""
                    });
                }
            }

            return View(categories);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        public IActionResult Create(ServiceCategory category)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO ServiceCategories (CategoryName, Remarks, Status) VALUES (@CategoryName, @Remarks, @Status)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    cmd.Parameters.AddWithValue("@Remarks", (object?)category.Remarks ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", category.Status);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            ServiceCategory category = new ServiceCategory();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM ServiceCategories WHERE CategoryId = @CategoryId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryId", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    category.CategoryName = reader["CategoryName"].ToString() ?? "";
                    category.Remarks = reader["Remarks"].ToString();
                    category.Status = reader["Status"].ToString() ?? "";
                }
            }

            return View(category);
        }

        // POST: Edit
        [HttpPost]
        public IActionResult Edit(ServiceCategory category)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE ServiceCategories SET CategoryName = @CategoryName, Remarks = @Remarks, Status = @Status WHERE CategoryId = @CategoryId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    cmd.Parameters.AddWithValue("@Remarks", (object?)category.Remarks ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", category.Status);
                    cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            ServiceCategory category = new ServiceCategory();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM ServiceCategories WHERE CategoryId = @CategoryId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryId", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    category.CategoryName = reader["CategoryName"].ToString() ?? "";
                    category.Remarks = reader["Remarks"].ToString();
                    category.Status = reader["Status"].ToString() ?? "";
                }
            }

            return View(category);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM ServiceCategories WHERE CategoryId = @CategoryId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryId", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
