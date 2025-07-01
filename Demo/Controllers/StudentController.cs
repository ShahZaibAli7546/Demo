using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _connectionString;

        public StudentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult Index()
        {
            List<Student> students = new List<Student>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Students", con);
            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                students.Add(new Student
                {
                    AcademicYearId = reader["AcademicYearId"] != DBNull.Value ? Convert.ToInt32(reader["AcademicYearId"]) : 0,
                    AdmissionDate = reader["AdmissionDate"] != DBNull.Value ? Convert.ToDateTime(reader["AdmissionDate"]) : DateTime.MinValue,
                    AdmissionNo = reader["AdmissionNo"]?.ToString() ?? "",
                    CourseId = reader["CourseId"] != DBNull.Value ? Convert.ToInt32(reader["CourseId"]) : 0,
                    StudentCategoryId = reader["StudentCategoryId"] != DBNull.Value ? Convert.ToInt32(reader["StudentCategoryId"]) : 0,

                    StudentName = reader["StudentName"]?.ToString() ?? "",
                    PersonalEmail = reader["PersonalEmail"]?.ToString() ?? "",
                    Gender = reader["Gender"]?.ToString() ?? "",
                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : DateTime.MinValue,
                    Nationality = reader["Nationality"]?.ToString() ?? "",
                    Religion = reader["Religion"]?.ToString() ?? "",
                    StudentMobile = reader["StudentMobile"]?.ToString() ?? "",
                    BirthPlace = reader["BirthPlace"]?.ToString() ?? "",
                    MotherTongue = reader["MotherTongue"]?.ToString() ?? "",

                    CNIC = reader["CNIC"]?.ToString() ?? "",
                    FatherName = reader["FatherName"]?.ToString() ?? "",
                    RelationToStudent = reader["RelationToStudent"]?.ToString() ?? "",
                    FatherOccupation = reader["FatherOccupation"]?.ToString() ?? "",
                    FatherMobile = reader["FatherMobile"]?.ToString() ?? "",
                    FatherEmail = reader["FatherEmail"]?.ToString() ?? "",
                    FatherAddressBuilding = reader["FatherAddressBuilding"]?.ToString() ?? "",
                    FatherStreetAddress = reader["FatherStreetAddress"]?.ToString() ?? "",
                    FatherCity = reader["FatherCity"]?.ToString() ?? "",
                    StudentMaritalStatus = reader["StudentMaritalStatus"]?.ToString() ?? "",

                    TuitionFeeId = reader["TuitionFeeId"] != DBNull.Value ? Convert.ToInt32(reader["TuitionFeeId"]) : 0,
                    DiscountId = reader["DiscountId"] != DBNull.Value ? Convert.ToInt32(reader["DiscountId"]) : (int?)null,
                    AfterDiscountAmount = reader["AfterDiscountAmount"] != DBNull.Value ? Convert.ToDecimal(reader["AfterDiscountAmount"]) : 0,
                    TransportFee = reader["TransportFee"] != DBNull.Value ? Convert.ToDecimal(reader["TransportFee"]) : (decimal?)null,
                    HostelFee = reader["HostelFee"] != DBNull.Value ? Convert.ToDecimal(reader["HostelFee"]) : (decimal?)null,
                    TotalFee = reader["TotalFee"] != DBNull.Value ? Convert.ToDecimal(reader["TotalFee"]) : 0
                });
            }

            return View(students);
        }

        public IActionResult Create()
        {
            var model = new Student();
            LoadDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(student);
                return View(student);
            }

            CalculateDiscountAmount(student);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Students (
                    AcademicYearId, AdmissionDate, AdmissionNo, CourseId, StudentCategoryId,
                    StudentName, PersonalEmail, Gender, DateOfBirth, Nationality, Religion, StudentMobile,
                    BirthPlace, MotherTongue, CNIC, FatherName, RelationToStudent, FatherOccupation,
                    FatherMobile, FatherEmail, FatherAddressBuilding, FatherStreetAddress, FatherCity,
                    StudentMaritalStatus, TuitionFeeId, DiscountId, AfterDiscountAmount,
                    TransportFee, HostelFee, TotalFee
                ) VALUES (
                    @AcademicYearId, @AdmissionDate, @AdmissionNo, @CourseId, @StudentCategoryId,
                    @StudentName, @PersonalEmail, @Gender, @DateOfBirth, @Nationality, @Religion, @StudentMobile,
                    @BirthPlace, @MotherTongue, @CNIC, @FatherName, @RelationToStudent, @FatherOccupation,
                    @FatherMobile, @FatherEmail, @FatherAddressBuilding, @FatherStreetAddress, @FatherCity,
                    @StudentMaritalStatus, @TuitionFeeId, @DiscountId, @AfterDiscountAmount,
                    @TransportFee, @HostelFee, @TotalFee
                )", con);

            AddParameters(student, cmd);
            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string admissionNo)
        {
            var student = GetStudentByAdmissionNo(admissionNo);
            if (student == null)
                return NotFound();

            LoadDropdowns(student);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(student);
                return View(student);
            }

            CalculateDiscountAmount(student);

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                UPDATE Students SET
                    AcademicYearId = @AcademicYearId, AdmissionDate = @AdmissionDate, CourseId = @CourseId,
                    StudentCategoryId = @StudentCategoryId, StudentName = @StudentName, PersonalEmail = @PersonalEmail,
                    Gender = @Gender, DateOfBirth = @DateOfBirth, Nationality = @Nationality, Religion = @Religion,
                    StudentMobile = @StudentMobile, BirthPlace = @BirthPlace, MotherTongue = @MotherTongue,
                    CNIC = @CNIC, FatherName = @FatherName, RelationToStudent = @RelationToStudent,
                    FatherOccupation = @FatherOccupation, FatherMobile = @FatherMobile, FatherEmail = @FatherEmail,
                    FatherAddressBuilding = @FatherAddressBuilding, FatherStreetAddress = @FatherStreetAddress,
                    FatherCity = @FatherCity, StudentMaritalStatus = @StudentMaritalStatus, TuitionFeeId = @TuitionFeeId,
                    DiscountId = @DiscountId, AfterDiscountAmount = @AfterDiscountAmount,
                    TransportFee = @TransportFee, HostelFee = @HostelFee, TotalFee = @TotalFee
                WHERE AdmissionNo = @AdmissionNo", con);

            AddParameters(student, cmd);
            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string admissionNo)
        {
            var student = GetStudentByAdmissionNo(admissionNo);
            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string admissionNo)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM Students WHERE AdmissionNo = @AdmissionNo", con);
            cmd.Parameters.AddWithValue("@AdmissionNo", admissionNo);
            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns(Student model)
        {
            model.AcademicYears = GetDropdownList("AcademicYears", "AcademicYearId", "AcademicYearName");
            model.Courses = GetDropdownList("Courses", "CourseId", "CourseName");
            model.StudentCategories = GetDropdownList("StudentCategory", "StudentCategoryId", "StudentCategoryName");
            model.Nationalities = GetDropdownList("Nationalities", "NationalityId", "NationalityName");
            model.Genders = new List<SelectListItem>
    {
        new("Male", "Male"), new("Female", "Female"), new("Other", "Other")
    };

            // 🔽 Updated to fetch FeeServices ordered by FeeServiceId DESC
            model.FeeServices = GetFeeServiceDropdownDescending();

            model.FeeDiscounts = GetFeeDiscountDropdown();
        }

        private List<SelectListItem> GetFeeServiceDropdownDescending()
        {
            var list = new List<SelectListItem>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT FeeServiceId, Cost FROM FeeServices ORDER BY FeeServiceId DESC", con);
            con.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = rdr["FeeServiceId"].ToString(),
                    Text = rdr["Cost"].ToString()
                });
            }
            return list;
        }


        private List<SelectListItem> GetDropdownList(string table, string idField, string textField)
        {
            var list = new List<SelectListItem>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand($"SELECT {idField}, {textField} FROM {table}", con);
            con.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = rdr[idField].ToString(),
                    Text = rdr[textField].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetFeeDiscountDropdown()
        {
            var list = new List<SelectListItem>();
            int latestId = 0;

            // Step 1: Get the latest discount ID
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT TOP 1 FeeDiscountId FROM FeeDiscount ORDER BY FeeDiscountId DESC", con))
            {
                con.Open();
                var result = cmd.ExecuteScalar();
                latestId = result != null ? Convert.ToInt32(result) : 0;
            }

            // Step 2: Build dropdown list and mark latest as selected
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT FeeDiscountId, ISNULL(Percentage, 0) AS Percentage, ISNULL(Amount, 0) AS Amount, 
               ISNULL(DiscountType, '') AS DiscountType
        FROM FeeDiscount", con))
            {
                con.Open();
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var id = Convert.ToInt32(rdr["FeeDiscountId"]);
                    var discountType = rdr["DiscountType"]?.ToString()?.Trim() ?? "";

                    string displayText = discountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase)
                        ? $"{Convert.ToDecimal(rdr["Percentage"]):0.##}%"
                        : $"{Convert.ToDecimal(rdr["Amount"]):0.##}";

                    list.Add(new SelectListItem
                    {
                        Value = id.ToString(),
                        Text = displayText,
                        Selected = id == latestId
                    });
                }
            }

            return list;
        }






        private Student? GetStudentByAdmissionNo(string admissionNo)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Students WHERE AdmissionNo = @AdmissionNo", con);
            cmd.Parameters.AddWithValue("@AdmissionNo", admissionNo);
            con.Open();
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Student
                {
                    AdmissionNo = rdr["AdmissionNo"].ToString(),
                    StudentName = rdr["StudentName"].ToString(),
                    Gender = rdr["Gender"].ToString(),
                    StudentMobile = rdr["StudentMobile"].ToString(),
                    AcademicYearId = Convert.ToInt32(rdr["AcademicYearId"]),
                    CourseId = Convert.ToInt32(rdr["CourseId"]),
                    TuitionFeeId = Convert.ToInt32(rdr["TuitionFeeId"]),
                    TotalFee = Convert.ToDecimal(rdr["TotalFee"])
                };
            }
            return null;
        }

        private void AddParameters(Student student, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@AcademicYearId", student.AcademicYearId);
            cmd.Parameters.AddWithValue("@AdmissionDate", student.AdmissionDate);
            cmd.Parameters.AddWithValue("@AdmissionNo", student.AdmissionNo ?? "");
            cmd.Parameters.AddWithValue("@CourseId", student.CourseId);
            cmd.Parameters.AddWithValue("@StudentCategoryId", student.StudentCategoryId);
            cmd.Parameters.AddWithValue("@StudentName", student.StudentName ?? "");
            cmd.Parameters.AddWithValue("@PersonalEmail", student.PersonalEmail ?? "");
            cmd.Parameters.AddWithValue("@Gender", student.Gender ?? "");
            cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
            cmd.Parameters.AddWithValue("@Nationality", student.Nationality ?? "");
            cmd.Parameters.AddWithValue("@Religion", student.Religion ?? "");
            cmd.Parameters.AddWithValue("@StudentMobile", student.StudentMobile ?? "");
            cmd.Parameters.AddWithValue("@BirthPlace", student.BirthPlace ?? "");
            cmd.Parameters.AddWithValue("@MotherTongue", student.MotherTongue ?? "");
            cmd.Parameters.AddWithValue("@CNIC", student.CNIC ?? "");
            cmd.Parameters.AddWithValue("@FatherName", student.FatherName ?? "");
            cmd.Parameters.AddWithValue("@RelationToStudent", student.RelationToStudent ?? "");
            cmd.Parameters.AddWithValue("@FatherOccupation", student.FatherOccupation ?? "");
            cmd.Parameters.AddWithValue("@FatherMobile", student.FatherMobile ?? "");
            cmd.Parameters.AddWithValue("@FatherEmail", student.FatherEmail ?? "");
            cmd.Parameters.AddWithValue("@FatherAddressBuilding", student.FatherAddressBuilding ?? "");
            cmd.Parameters.AddWithValue("@FatherStreetAddress", student.FatherStreetAddress ?? "");
            cmd.Parameters.AddWithValue("@FatherCity", student.FatherCity ?? "");
            cmd.Parameters.AddWithValue("@StudentMaritalStatus", student.StudentMaritalStatus ?? "");
            cmd.Parameters.AddWithValue("@TuitionFeeId", student.TuitionFeeId);
            cmd.Parameters.AddWithValue("@DiscountId", (object?)student.DiscountId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AfterDiscountAmount", student.AfterDiscountAmount);
            cmd.Parameters.AddWithValue("@TransportFee", (object?)student.TransportFee ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HostelFee", (object?)student.HostelFee ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TotalFee", student.TotalFee);
        }

        private void CalculateDiscountAmount(Student student)
        {
            decimal tuitionFee = 0;
            decimal discountValue = 0;

            // Step 1: Get the tuition fee amount based on TuitionFeeId
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT Cost FROM FeeServices WHERE FeeServiceId = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", student.TuitionFeeId);
                con.Open();
                var result = cmd.ExecuteScalar();
                tuitionFee = result != null ? Convert.ToDecimal(result) : 0;
            }

            // Step 2: If a discount is selected, apply it
            if (student.DiscountId.HasValue)
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
            SELECT ISNULL(Percentage, 0) AS Percentage, 
                   ISNULL(Amount, 0) AS Amount, 
                   DiscountType
            FROM FeeDiscount
            WHERE FeeDiscountId = @Id", con);

                cmd.Parameters.AddWithValue("@Id", student.DiscountId.Value);
                con.Open();

                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var discountType = rdr["DiscountType"]?.ToString()?.Trim() ?? "";

                    if (discountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                    {
                        var percentage = Convert.ToDecimal(rdr["Percentage"]);
                        discountValue = (tuitionFee * percentage) / 100;
                    }
                    else if (discountType.Equals("Fixed Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        discountValue = Convert.ToDecimal(rdr["Amount"]);
                    }
                }
            }

            // Step 3: Calculate final fee after discount
            student.AfterDiscountAmount = tuitionFee - discountValue;

            // Optional (if needed): Store TotalFee including Transport/Hostel fees
            student.TotalFee = student.AfterDiscountAmount +
                               (student.TransportFee ?? 0) +
                               (student.HostelFee ?? 0);
        }

    }
}
