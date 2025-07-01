using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Demo.Models;
using System.Data;

namespace Demo.Controllers
{
    public class UpdateTuitionFeeController : Controller
    {
        private readonly string connectionString;

        public UpdateTuitionFeeController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        public IActionResult Index(int? campusId, int? batchId)
        {
            var campuses = GetCampuses();
            if (!campusId.HasValue && campuses.Any())
                campusId = int.Parse(campuses.First().Value);

            var model = new TuitionFeeBatchViewModel
            {
                SelectedCampusId = campusId ?? 0,
                SelectedBatchId = batchId ?? 0,
                Students = batchId.HasValue ? GetStudentsByBatch(batchId.Value) : new List<UpdateTuitionFee>()
            };

            ViewBag.Campuses = campuses;
            ViewBag.Batches = GetBatches(campusId);
            ViewBag.FeeServices = GetFeeServices();
            ViewBag.Discounts = GetDiscounts();

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(TuitionFeeBatchViewModel model)
        {
            if (model.Students != null && model.Students.Count > 0)
            {
                using SqlConnection con = new(connectionString);
                con.Open();

                foreach (var student in model.Students)
                {
                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE Students
                        SET FeeServiceId = @FeeServiceId,
                            DiscountId = @DiscountId,
                            Amount = @Amount
                        WHERE StudentId = @StudentId", con);

                    cmd.Parameters.AddWithValue("@FeeServiceId", (object?)student.FeeServiceId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiscountId", (object?)student.DiscountId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", student.Amount);
                    cmd.Parameters.AddWithValue("@StudentId", student.StudentId);

                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Tuition fee updated successfully!";
                return RedirectToAction("Index", new { campusId = model.SelectedCampusId, batchId = model.SelectedBatchId });
            }

            TempData["ErrorMessage"] = "No students found to update.";

            ViewBag.Campuses = GetCampuses();
            ViewBag.Batches = GetBatches(model.SelectedCampusId);
            ViewBag.FeeServices = GetFeeServices();
            ViewBag.Discounts = GetDiscounts();

            return View("Index", model);
        }

        private List<UpdateTuitionFee> GetStudentsByBatch(int batchId)
        {
            var students = new List<UpdateTuitionFee>();

            using SqlConnection con = new(connectionString);
            SqlCommand cmd = new("SELECT StudentId, StudentName, FatherName, FeeServiceId, DiscountId, Amount FROM Students WHERE BatchId = @BatchId", con);
            cmd.Parameters.AddWithValue("@BatchId", batchId);
            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new UpdateTuitionFee
                {
                    StudentId = (int)reader["StudentId"],
                    StudentName = reader["StudentName"]?.ToString() ?? "",
                    FatherName = reader["FatherName"]?.ToString() ?? "",
                    FeeServiceId = reader["FeeServiceId"] != DBNull.Value ? (int?)reader["FeeServiceId"] : null,
                    DiscountId = reader["DiscountId"] != DBNull.Value ? (int?)reader["DiscountId"] : null,
                    Amount = reader["Amount"] != DBNull.Value ? (decimal)reader["Amount"] : 0
                });
            }

            return students;
        }

        private List<SelectListItem> GetCampuses()
        {
            var list = new List<SelectListItem>();

            using SqlConnection con = new(connectionString);
            SqlCommand cmd = new("SELECT CampusId, CampusName FROM Campuses", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["CampusId"].ToString(),
                    Text = reader["CampusName"].ToString()
                });
            }

            return list;
        }

        private List<SelectListItem> GetBatches(int? campusId = null)
        {
            var list = new List<SelectListItem>();

            using SqlConnection con = new(connectionString);
            SqlCommand cmd = new("SELECT BatchId, BatchName FROM Batches", con);

            if (campusId.HasValue)
            {
                cmd.CommandText += " WHERE CampusId = @CampusId";
                cmd.Parameters.AddWithValue("@CampusId", campusId.Value);
            }

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["BatchId"].ToString(),
                    Text = reader["BatchName"].ToString()
                });
            }

            return list;
        }

        private List<SelectListItem> GetFeeServices()
        {
            var list = new List<SelectListItem>();

            using SqlConnection con = new(connectionString);
            SqlCommand cmd = new("SELECT FeeServiceId, FeeServiceName FROM FeeServices", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["FeeServiceId"].ToString(),
                    Text = reader["FeeServiceName"].ToString()
                });
            }

            return list;
        }

        private List<SelectListItem> GetDiscounts()
        {
            var list = new List<SelectListItem>();

            using SqlConnection con = new(connectionString);
            SqlCommand cmd = new("SELECT DiscountId, DiscountTitle FROM Discounts", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["DiscountId"].ToString(),
                    Text = reader["DiscountTitle"].ToString()
                });
            }

            return list;
        }
    }
}
