using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class UpdateTransportFeeController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            List<UpdateTransportFee> fees = [];

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM UpdateTransportFee", con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                fees.Add(new UpdateTransportFee
                {
                    Id = reader.GetInt32(0),
                    CampusId = reader.GetInt32(1),
                    BatchId = reader.GetInt32(2),
                    FeeServiceId = reader.GetInt32(3),
                    SelectType = reader["SelectType"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Status = reader["Status"].ToString() ?? "Inactive"
                });
            }

            return View(fees);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(UpdateTransportFee model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO UpdateTransportFee 
                (CampusId, BatchId, FeeServiceId, SelectType, Amount, Status)
                VALUES 
                (@CampusId, @BatchId, @FeeServiceId, @SelectType, @Amount, @Status)", con);

            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
            cmd.Parameters.AddWithValue("@BatchId", model.BatchId);
            cmd.Parameters.AddWithValue("@FeeServiceId", model.FeeServiceId);
            cmd.Parameters.AddWithValue("@SelectType", model.SelectType);
            cmd.Parameters.AddWithValue("@Amount", model.Amount);
            cmd.Parameters.AddWithValue("@Status", model.Status);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "✅ Transport fee added.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            UpdateTransportFee? fee = null;

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM UpdateTransportFee WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                fee = new UpdateTransportFee
                {
                    Id = reader.GetInt32(0),
                    CampusId = reader.GetInt32(1),
                    BatchId = reader.GetInt32(2),
                    FeeServiceId = reader.GetInt32(3),
                    SelectType = reader["SelectType"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Status = reader["Status"].ToString() ?? "Inactive"
                };
            }

            if (fee == null)
                return NotFound();

            LoadDropdowns();
            return View(fee);
        }

        [HttpPost]
        public IActionResult Edit(UpdateTransportFee model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE UpdateTransportFee SET 
                    CampusId = @CampusId,
                    BatchId = @BatchId,
                    FeeServiceId = @FeeServiceId,
                    SelectType = @SelectType,
                    Amount = @Amount,
                    Status = @Status
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
            cmd.Parameters.AddWithValue("@BatchId", model.BatchId);
            cmd.Parameters.AddWithValue("@FeeServiceId", model.FeeServiceId);
            cmd.Parameters.AddWithValue("@SelectType", model.SelectType);
            cmd.Parameters.AddWithValue("@Amount", model.Amount);
            cmd.Parameters.AddWithValue("@Status", model.Status);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "✅ Transport fee updated.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            UpdateTransportFee? fee = null;

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM UpdateTransportFee WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                fee = new UpdateTransportFee
                {
                    Id = reader.GetInt32(0),
                    CampusId = reader.GetInt32(1),
                    BatchId = reader.GetInt32(2),
                    FeeServiceId = reader.GetInt32(3),
                    SelectType = reader["SelectType"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Status = reader["Status"].ToString() ?? "Inactive"
                };
            }

            if (fee == null)
                return NotFound();

            return View(fee);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM UpdateTransportFee WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "✅ Transport fee deleted.";
            return RedirectToAction("Index");
        }

        // 🔁 Auto-fetch Fee Services (general list - no batch filtering)
        public JsonResult GetFeeServicesByBatch(int batchId)
        {
            var services = new List<SelectListItem>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT Id, FeeServiceName FROM FeeServices WHERE Status = 'Active'", con); // ✅ Removed BatchId filter
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                services.Add(new SelectListItem
                {
                    Value = reader["Id"].ToString(),
                    Text = reader["FeeServiceName"].ToString()
                });
            }

            return Json(services);
        }


        private void LoadDropdowns()
        {
            ViewBag.Campuses = GetSelectList("SELECT CampusId, CampusName FROM Campuses");
            ViewBag.Batches = GetSelectList("SELECT BatchId, BatchName FROM Batches");

            ViewBag.FeeServices = GetSelectList("SELECT Id, FeeServiceName FROM FeeServices WHERE Status = 'Active'");

            ViewBag.SelectTypes = new List<SelectListItem>
    {
        new("Amount Base", "Amount Base"),
        new("Percentage Base", "Percentage Base")
    };

            ViewBag.Statuses = new List<SelectListItem>
    {
        new("Active", "Active"),
        new("Inactive", "Inactive")
    };
        }


        private List<SelectListItem> GetSelectList(string query)
        {
            List<SelectListItem> items = [];
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new SelectListItem
                {
                    Value = reader.GetValue(0).ToString(),
                    Text = reader.GetString(1)
                });
            }
            return items;
        }
    }
}
