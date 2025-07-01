using Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Demo.Controllers
{
    public class SchoolGeneralSettingsController : Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;

        public SchoolGeneralSettingsController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _env = env;
        }

        public IActionResult Index()
        {
            SchoolGeneralSettings model = new();
            using SqlConnection con = new(_connectionString);
            con.Open();

            SqlCommand cmd = new("SELECT TOP 1 * FROM SchoolGeneralSettings", con);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.Id = Convert.ToInt32(reader["Id"]);
                model.SchoolName = reader["SchoolName"]?.ToString();
                model.InstitutionType = reader["InstitutionType"]?.ToString();
                model.SchoolAbbreviation = reader["SchoolAbbreviation"]?.ToString();
                model.Address = reader["Address"]?.ToString();
                model.BankName = reader["BankName"]?.ToString();
                model.AccountNumber = reader["AccountNumber"]?.ToString();
                model.IbanNumber = reader["IbanNumber"]?.ToString();
                model.AdmissionNoPrefix = reader["AdmissionNoPrefix"]?.ToString();
                model.AdmissionNoPostfix = reader["AdmissionNoPostfix"]?.ToString();
                model.InstitutionFeeCriteria = reader["InstitutionFeeCriteria"]?.ToString();
                model.PreAdmissionNoPrefix = reader["PreAdmissionNoPrefix"]?.ToString();
                model.PreAdmissionNoPostfix = reader["PreAdmissionNoPostfix"]?.ToString();
                model.PaymentReceiptFormat = reader["PaymentReceiptFormat"]?.ToString();
                model.FamilyWisePaymentReceiptFormat = reader["FamilyWisePaymentReceiptFormat"]?.ToString();
                model.AdmissionFormFormat = reader["AdmissionFormFormat"]?.ToString();
                model.ReceiptsAndPaymentsFormat = reader["ReceiptsAndPaymentsFormat"]?.ToString();
                model.AssessmentTypeForApp = reader["AssessmentTypeForApp"]?.ToString();
                model.CreateFeeVoucherWithZeroAmount = Convert.ToBoolean(reader["CreateFeeVoucherWithZeroAmount"]);
                model.InactiveBatchTimetableValidation = Convert.ToBoolean(reader["InactiveBatchTimetableValidation"]);
                model.EmployeeIdPrefix = reader["EmployeeIdPrefix"]?.ToString();
                model.EmployeeIdPostfix = reader["EmployeeIdPostfix"]?.ToString();
                model.RetirementAge = reader["RetirementAge"] as int?;
                model.SecurityPaymentReceiptFormat = reader["SecurityPaymentReceiptFormat"]?.ToString();
                model.GateAttendanceType = reader["GateAttendanceType"]?.ToString();
                model.FeeVoucherFormat = reader["FeeVoucherFormat"]?.ToString();
                model.ServiceType = reader["ServiceType"]?.ToString();
                model.BalanceChoice = reader["BalanceChoice"]?.ToString();
                model.FineType = reader["FineType"]?.ToString();
                model.DiscountType = reader["DiscountType"]?.ToString();
                model.MessageOnFeeReceive = Convert.ToBoolean(reader["MessageOnFeeReceive"]);
                model.MessageOnInquiry = Convert.ToBoolean(reader["MessageOnInquiry"]);
                model.MessageOnAdmission = Convert.ToBoolean(reader["MessageOnAdmission"]);
                model.MessageOnFeeReceiptInstallment = Convert.ToBoolean(reader["MessageOnFeeReceiptInstallment"]);
                model.MessageOnDailyAttendance = Convert.ToBoolean(reader["MessageOnDailyAttendance"]);
                model.MessageOfAttendanceFromTeacher = Convert.ToBoolean(reader["MessageOfAttendanceFromTeacher"]);
                model.AppNotificationOnComplainStatusChange = Convert.ToBoolean(reader["AppNotificationOnComplainStatusChange"]);
                model.AppNotificationOnDateAttendance = Convert.ToBoolean(reader["AppNotificationOnDateAttendance"]);
                model.AppNotificationOnStudentAttendanceSubjectBasis = Convert.ToBoolean(reader["AppNotificationOnStudentAttendanceSubjectBasis"]);
                model.SmsNotificationOnDateAttendance = Convert.ToBoolean(reader["SmsNotificationOnDateAttendance"]);
                model.AppNotificationOnNewAnnouncement = Convert.ToBoolean(reader["AppNotificationOnNewAnnouncement"]);
                model.AppNotificationOnBatchActivity = Convert.ToBoolean(reader["AppNotificationOnBatchActivity"]);
                model.AppNotificationOnMeeting = Convert.ToBoolean(reader["AppNotificationOnMeeting"]);
                model.AppNotificationOnApplication = Convert.ToBoolean(reader["AppNotificationOnApplication"]);
                model.Logo1Path = reader["Logo1Path"]?.ToString();
                model.Logo2Path = reader["Logo2Path"]?.ToString();
                model.PaidStampPath = reader["PaidStampPath"]?.ToString();
                model.ReportHeaderPath = reader["ReportHeaderPath"]?.ToString();
                model.ReportCardBackgroundPath = reader["ReportCardBackgroundPath"]?.ToString();
                model.PrincipalSignatureLogoPath = reader["PrincipalSignaturePath"]?.ToString();
            }
            con.Close();

            ViewBag.FeeCriteriaList = GetFeeCriteriaOptions(); // ✅ corrected ViewBag name
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(SchoolGeneralSettings model, IFormFile? Logo1File, IFormFile? Logo2File,
            IFormFile? PaidStampFile, IFormFile? ReportHeaderFile, IFormFile? ReportCardBackgroundFile,
            IFormFile? PrincipalSignatureLogoFile)
        {
            string UploadFile(IFormFile? file, string name)
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = $"{name}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
                    string filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    file.CopyTo(stream);
                    return "/uploads/" + fileName;
                }
                return "";
            }

            model.Logo1Path = UploadFile(Logo1File, "logo1");
            model.Logo2Path = UploadFile(Logo2File, "logo2");
            model.PaidStampPath = UploadFile(PaidStampFile, "paidstamp");
            model.ReportHeaderPath = UploadFile(ReportHeaderFile, "reportheader");
            model.ReportCardBackgroundPath = UploadFile(ReportCardBackgroundFile, "reportcardbg");
            model.PrincipalSignatureLogoPath = UploadFile(PrincipalSignatureLogoFile, "principalsign");

            using SqlConnection con = new(_connectionString);
            SqlCommand cmd;

            if (model.Id > 0)
            {
                cmd = new SqlCommand("UPDATE SchoolGeneralSettings SET " +
                    // 🔹 All columns here...
                    "PrincipalSignaturePath=@PrincipalSignaturePath WHERE Id=@Id", con);
            }
            else
            {
                cmd = new SqlCommand("INSERT INTO SchoolGeneralSettings (...columns...) VALUES (...values...)", con);
            }

            foreach (var prop in typeof(SchoolGeneralSettings).GetProperties())
            {
                var value = prop.GetValue(model) ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@" + prop.Name, value);
            }

            con.Open();
            cmd.ExecuteNonQuery();
            TempData["Message"] = "School General Settings saved successfully!";
            return RedirectToAction("Index");
        }

        // 🔸 Dropdown Loader
        private List<SelectListItem> GetFeeCriteriaOptions()
        {
            var list = new List<SelectListItem>();
            using SqlConnection con = new(_connectionString);
            con.Open();

            using (SqlCommand cmd1 = new("SELECT DISTINCT FeeServiceName FROM FeeServices", con))
            using (SqlDataReader reader1 = cmd1.ExecuteReader())
            {
                while (reader1.Read())
                {
                    if (!reader1.IsDBNull(0))
                    {
                        string value = reader1.GetString(0);
                        list.Add(new SelectListItem { Text = "FeeServices: " + value, Value = value });
                    }
                }
            }

            using (SqlCommand cmd2 = new("SELECT DISTINCT StructureName FROM FeeStructure", con))
            using (SqlDataReader reader2 = cmd2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    if (!reader2.IsDBNull(0))
                    {
                        string value = reader2.GetString(0);
                        list.Add(new SelectListItem { Text = "FeeStructure: " + value, Value = value });
                    }
                }
            }

            return list;
        }
    }
}
