using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class RecurringPayslipController : Controller
    {
        private readonly string _connectionString;

        public RecurringPayslipController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // 🔷 INDEX
        public IActionResult Index()
        {
            List<RecurringPayslip> payslips = new();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"SELECT rp.Id, rp.CampusId, c.CampusName, rp.EmployeeId, e.EmployeeName, 
                                                rp.RateType, rp.LoanReturn, rp.Description, 
                                                rp.GrossPay, rp.TotalDeduction, rp.TotalContribution 
                                         FROM RecurringPayslips rp
                                         INNER JOIN Campuses c ON rp.CampusId = c.CampusId
                                         INNER JOIN Employees e ON rp.EmployeeId = e.EmployeeId", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                payslips.Add(new RecurringPayslip
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CampusId = Convert.ToInt32(reader["CampusId"]),
                    CampusName = reader["CampusName"].ToString() ?? "",
                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                    EmployeeName = reader["EmployeeName"].ToString() ?? "",
                    RateType = reader["RateType"].ToString() ?? "",
                    LoanReturn = Convert.ToBoolean(reader["LoanReturn"]),
                    Description = reader["Description"].ToString() ?? "",
                    GrossPay = Convert.ToDecimal(reader["GrossPay"]),
                    TotalDeduction = Convert.ToDecimal(reader["TotalDeduction"]),
                    TotalContribution = Convert.ToDecimal(reader["TotalContribution"])
                });
            }

            return View(payslips);
        }

        // 🔹 GET: CREATE
        public IActionResult Create()
        {
            ViewBag.Campuses = GetCampuses();
            ViewBag.Employees = new List<SelectListItem>();
            ViewBag.EarningItems = GetEarningItems();
            ViewBag.DeductionItems = GetDeductionItems();
            ViewBag.ContributionItems = GetContributionItems();

            return View(new RecurringPayslip());
        }

        // 🔹 POST: CREATE
        [HttpPost]
        public IActionResult Create(RecurringPayslip model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Campuses = GetCampuses();
                ViewBag.Employees = GetEmployees(model.CampusId);
                ViewBag.EarningItems = GetEarningItems();
                ViewBag.DeductionItems = GetDeductionItems();
                ViewBag.ContributionItems = GetContributionItems();
                return View(model);
            }

            using SqlConnection con = new(_connectionString);
            con.Open();

            int newId;
            using (SqlCommand cmd = new(@"INSERT INTO RecurringPayslips (CampusId, EmployeeId, RateType, LoanReturn, Description, GrossPay, TotalDeduction, TotalContribution)
                                          OUTPUT INSERTED.Id
                                          VALUES (@CampusId, @EmployeeId, @RateType, @LoanReturn, @Description, @GrossPay, @TotalDeduction, @TotalContribution)", con))
            {
                cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
                cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
                cmd.Parameters.AddWithValue("@RateType", model.RateType);
                cmd.Parameters.AddWithValue("@LoanReturn", model.LoanReturn);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@GrossPay", model.GrossPay);
                cmd.Parameters.AddWithValue("@TotalDeduction", model.TotalDeduction);
                cmd.Parameters.AddWithValue("@TotalContribution", model.TotalContribution);

                newId = (int)cmd.ExecuteScalar();
            }

            model.Id = newId;
            InsertRecurringItems(con, model);

            return RedirectToAction("Index");
        }

        // 🔹 GET: EDIT
        public IActionResult Edit(int id)
        {
            RecurringPayslip? model = null;

            using SqlConnection con = new(_connectionString);
            con.Open();

            using (SqlCommand cmd = new("SELECT * FROM RecurringPayslips WHERE Id = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new RecurringPayslip
                    {
                        Id = id,
                        CampusId = Convert.ToInt32(reader["CampusId"]),
                        EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                        RateType = reader["RateType"].ToString() ?? "",
                        LoanReturn = Convert.ToBoolean(reader["LoanReturn"]),
                        Description = reader["Description"].ToString() ?? "",
                        GrossPay = Convert.ToDecimal(reader["GrossPay"]),
                        TotalDeduction = Convert.ToDecimal(reader["TotalDeduction"]),
                        TotalContribution = Convert.ToDecimal(reader["TotalContribution"]),
                        EarningItems = new(),
                        DeductionItems = new(),
                        ContributionItems = new()
                    };
                }
            }

            if (model == null) return NotFound();

            model.EarningItems = LoadEarnings(id);
            model.DeductionItems = LoadDeductions(id);
            model.ContributionItems = LoadContributions(id);

            ViewBag.Campuses = GetCampuses();
            ViewBag.Employees = GetEmployees(model.CampusId);
            ViewBag.EarningItems = GetEarningItems();
            ViewBag.DeductionItems = GetDeductionItems();
            ViewBag.ContributionItems = GetContributionItems();

            return View(model);
        }

        // 🔹 POST: EDIT
        [HttpPost]
        public IActionResult Edit(RecurringPayslip model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Campuses = GetCampuses();
                ViewBag.Employees = GetEmployees(model.CampusId);
                ViewBag.EarningItems = GetEarningItems();
                ViewBag.DeductionItems = GetDeductionItems();
                ViewBag.ContributionItems = GetContributionItems();
                return View(model);
            }

            using SqlConnection con = new(_connectionString);
            con.Open();

            using (SqlCommand cmd = new(
                "UPDATE RecurringPayslips SET CampusId = @CampusId, EmployeeId = @EmployeeId, RateType = @RateType, LoanReturn = @LoanReturn, " +
                "Description = @Description, GrossPay = @GrossPay, TotalDeduction = @TotalDeduction, TotalContribution = @TotalContribution " +
                "WHERE Id = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@CampusId", model.CampusId);
                cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
                cmd.Parameters.AddWithValue("@RateType", model.RateType);
                cmd.Parameters.AddWithValue("@LoanReturn", model.LoanReturn);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@GrossPay", model.GrossPay);
                cmd.Parameters.AddWithValue("@TotalDeduction", model.TotalDeduction);
                cmd.Parameters.AddWithValue("@TotalContribution", model.TotalContribution);
                cmd.ExecuteNonQuery();
            }

            DeleteRecurringItems(con, model.Id);
            InsertRecurringItems(con, model);

            return RedirectToAction("Index");
        }

        // 🔴 DELETE
        public IActionResult Delete(int id)
        {
            using SqlConnection con = new(_connectionString);
            con.Open();

            DeleteRecurringItems(con, id);

            using SqlCommand cmd = new("DELETE FROM RecurringPayslips WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        // 🔸 AJAX: Get Employees by Campus
        [HttpGet]
        public JsonResult GetEmployeesByCampus(int campusId)
        {
            List<SelectListItem> employees = new();
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("SELECT EmployeeId, EmployeeName FROM Employees WHERE CampusId = @CampusId", con);
            cmd.Parameters.AddWithValue("@CampusId", campusId);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(new SelectListItem
                {
                    Value = reader["EmployeeId"].ToString(),
                    Text = reader["EmployeeName"].ToString()
                });
            }
            return Json(employees);
        }


        // 🔧 Helpers
        private List<SelectListItem> GetCampuses()
        {
            List<SelectListItem> list = new();
            using SqlConnection con = new(_connectionString);
            con.Open();
            using SqlCommand cmd = new("SELECT CampusId, CampusName FROM Campuses", con);
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

        private List<SelectListItem> GetEmployees(int campusId)
        {
            List<SelectListItem> list = new();
            using SqlConnection con = new(_connectionString);
            con.Open();
            using SqlCommand cmd = new("SELECT EmployeeId, EmployeeName FROM Employees WHERE CampusId = @CampusId", con);
            cmd.Parameters.AddWithValue("@CampusId", campusId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["EmployeeId"].ToString(),
                    Text = reader["EmployeeName"].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetEarningItems()
        {
            return GetSimpleList("PayslipEarningItems");
        }

        private List<SelectListItem> GetDeductionItems()
        {
            return GetSimpleList("PayslipDeductionItems");
        }

        private List<SelectListItem> GetContributionItems()
        {
            return GetSimpleList("PayslipContributionItems");
        }

        private List<SelectListItem> GetSimpleList(string tableName)
        {
            List<SelectListItem> list = new();
            using SqlConnection con = new(_connectionString);
            con.Open();
            using SqlCommand cmd = new($"SELECT Id, Title FROM {tableName}", con);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["Id"].ToString(),
                    Text = reader["Title"].ToString()
                });
            }
            return list;
        }

        private void InsertRecurringItems(SqlConnection con, RecurringPayslip model)
        {
            foreach (var item in model.EarningItems)
            {
                using SqlCommand cmd = new("INSERT INTO RecurringPayslipEarningItems (RecurringPayslipId, EarningAccountId, Rate, Days, Hours, Minutes, Description, Amount) VALUES (@RecurringPayslipId, @EarningAccountId, @Rate, @Days, @Hours, @Minutes, @Description, @Amount)", con);
                cmd.Parameters.AddWithValue("@RecurringPayslipId", model.Id);
                cmd.Parameters.AddWithValue("@EarningAccountId", item.EarningAccountId);
                cmd.Parameters.AddWithValue("@Rate", item.Rate);
                cmd.Parameters.AddWithValue("@Days", item.Days);
                cmd.Parameters.AddWithValue("@Hours", item.Hours);
                cmd.Parameters.AddWithValue("@Minutes", item.Minutes);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? "");
                cmd.Parameters.AddWithValue("@Amount", item.Amount);
                cmd.ExecuteNonQuery();
            }

            foreach (var item in model.DeductionItems)
            {
                using SqlCommand cmd = new("INSERT INTO RecurringPayslipDeductionItems (RecurringPayslipId, DeductionAccountId, Description, Amount, DeductionType) VALUES (@RecurringPayslipId, @DeductionAccountId, @Description, @Amount, @DeductionType)", con);
                cmd.Parameters.AddWithValue("@RecurringPayslipId", model.Id);
                cmd.Parameters.AddWithValue("@DeductionAccountId", item.DeductionAccountId);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? "");
                cmd.Parameters.AddWithValue("@Amount", item.Amount);
                cmd.Parameters.AddWithValue("@DeductionType", item.DeductionType ?? "Absolute");
                cmd.ExecuteNonQuery();
            }

            foreach (var item in model.ContributionItems)
            {
                using SqlCommand cmd = new("INSERT INTO RecurringPayslipContributionItems (RecurringPayslipId, ContributionAccountId, Description, Amount, ContributionType, IsApplicableEveryMonth) VALUES (@RecurringPayslipId, @ContributionAccountId, @Description, @Amount, @ContributionType, @IsApplicableEveryMonth)", con);
                cmd.Parameters.AddWithValue("@RecurringPayslipId", model.Id);
                cmd.Parameters.AddWithValue("@ContributionAccountId", item.ContributionAccountId);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? "");
                cmd.Parameters.AddWithValue("@Amount", item.Amount);
                cmd.Parameters.AddWithValue("@ContributionType", item.ContributionType ?? "Absolute");
                cmd.Parameters.AddWithValue("@IsApplicableEveryMonth", item.IsApplicableEveryMonth);
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteRecurringItems(SqlConnection con, int id)
        {
            string[] tables = {
                "RecurringPayslipEarningItems",
                "RecurringPayslipDeductionItems",
                "RecurringPayslipContributionItems"
            };

            foreach (var table in tables)
            {
                using SqlCommand cmd = new($"DELETE FROM {table} WHERE RecurringPayslipId = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private List<RecurringPayslipEarningItem> LoadEarnings(int id)
        {
            return LoadItems<RecurringPayslipEarningItem>(id, "RecurringPayslipEarningItems", reader => new RecurringPayslipEarningItem
            {
                EarningAccountId = Convert.ToInt32(reader["EarningAccountId"]),
                Rate = Convert.ToDecimal(reader["Rate"]),
                Days = Convert.ToInt32(reader["Days"]),
                Hours = Convert.ToInt32(reader["Hours"]),
                Minutes = Convert.ToInt32(reader["Minutes"]),
                Description = reader["Description"].ToString() ?? "",
                Amount = Convert.ToDecimal(reader["Amount"])
            });
        }

        private List<RecurringPayslipDeductionItem> LoadDeductions(int id)
        {
            return LoadItems<RecurringPayslipDeductionItem>(id, "RecurringPayslipDeductionItems", reader => new RecurringPayslipDeductionItem
            {
                DeductionAccountId = Convert.ToInt32(reader["DeductionAccountId"]),
                Description = reader["Description"].ToString() ?? "",
                Amount = Convert.ToDecimal(reader["Amount"]),
                DeductionType = reader["DeductionType"].ToString() ?? "Absolute"
            });
        }

        private List<RecurringPayslipContributionItem> LoadContributions(int id)
        {
            return LoadItems<RecurringPayslipContributionItem>(id, "RecurringPayslipContributionItems", reader => new RecurringPayslipContributionItem
            {
                ContributionAccountId = Convert.ToInt32(reader["ContributionAccountId"]),
                Description = reader["Description"].ToString() ?? "",
                Amount = Convert.ToDecimal(reader["Amount"]),
                ContributionType = reader["ContributionType"].ToString() ?? "Absolute",
                IsApplicableEveryMonth = Convert.ToBoolean(reader["IsApplicableEveryMonth"])
            });
        }

        private List<T> LoadItems<T>(int id, string table, Func<SqlDataReader, T> map)
        {
            List<T> list = new();
            using SqlConnection con = new(_connectionString);
            con.Open();
            using SqlCommand cmd = new($"SELECT * FROM {table} WHERE RecurringPayslipId = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(map(reader));
            return list;
        }
    }
}
