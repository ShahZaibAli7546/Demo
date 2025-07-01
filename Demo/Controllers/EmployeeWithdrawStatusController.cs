using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class EmployeeWithdrawStatusController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeeWithdrawStatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ INDEX
        public IActionResult Index()
        {
            List<EmployeeWithdrawStatus> list = new();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new("SELECT * FROM EmployeeWithdrawStatus", con);
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new EmployeeWithdrawStatus
                {
                    Id = Convert.ToInt32(rdr["Id"]),
                    Title = rdr["Title"].ToString()!,
                    Reason = rdr["Reason"].ToString()!,
                    Description = rdr["Description"].ToString(),
                    Status = rdr["Status"].ToString()!
                });
            }
            return View(list);
        }

        // ✅ CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        // ✅ CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeWithdrawStatus model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string query = "INSERT INTO EmployeeWithdrawStatus (Title, Reason, Description, Status) VALUES (@Title, @Reason, @Description, @Status)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@Reason", model.Reason);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Withdraw status added successfully!";
            return RedirectToAction("Index");
        }

        // ✅ EDIT GET
        public IActionResult Edit(int id)
        {
            EmployeeWithdrawStatus model = new();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new("SELECT * FROM EmployeeWithdrawStatus WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                model.Id = Convert.ToInt32(rdr["Id"]);
                model.Title = rdr["Title"].ToString()!;
                model.Reason = rdr["Reason"].ToString()!;
                model.Description = rdr["Description"].ToString();
                model.Status = rdr["Status"].ToString()!;
            }
            return View(model);
        }

        // ✅ EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeWithdrawStatus model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string query = "UPDATE EmployeeWithdrawStatus SET Title = @Title, Reason = @Reason, Description = @Description, Status = @Status WHERE Id = @Id";
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@Reason", model.Reason);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Withdraw status updated!";
            return RedirectToAction("Index");
        }

        // ✅ DELETE GET
        public IActionResult Delete(int id)
        {
            EmployeeWithdrawStatus model = new();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new("SELECT * FROM EmployeeWithdrawStatus WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                model.Id = Convert.ToInt32(rdr["Id"]);
                model.Title = rdr["Title"].ToString()!;
                model.Reason = rdr["Reason"].ToString()!;
                model.Description = rdr["Description"].ToString();
                model.Status = rdr["Status"].ToString()!;
            }
            return View(model);
        }

        // ✅ DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string query = "DELETE FROM EmployeeWithdrawStatus WHERE Id = @Id";
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Withdraw status deleted.";
            return RedirectToAction("Index");
        }
    }
}
