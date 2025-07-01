using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Demo.Controllers
{
    public class ClassroomController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ClassroomController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        // 🔷 INDEX
        public IActionResult Index()
        {
            List<Classroom> classrooms = new List<Classroom>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Classroom";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    classrooms.Add(new Classroom
                    {
                        ClassId = Convert.ToInt32(reader["ClassId"]),
                        ClassName = reader["ClassName"]?.ToString() ?? "",
                        Status = reader["Status"]?.ToString() ?? "",
                        QRCodeLink = reader["QRCodeLink"]?.ToString() ?? ""
                    });
                }
            }

            return View(classrooms);
        }

        // 🔷 CREATE GET
        public IActionResult Create()
        {
            var model = new Classroom
            {
                Status = "Active" // 👈 Optional default
            };
            return View(model); // 👈 MUST pass model
        }


        // 🔷 CREATE POST
        [HttpPost]
        public IActionResult Create(Classroom model)
        {
            // Generate QRCodeLink before model validation
            model.QRCodeLink = $"https://yourdomain.com/classroom/{Guid.NewGuid()}";

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        string query = "INSERT INTO Classroom (ClassName, Status, QRCodeLink) VALUES (@ClassName, @Status, @QRCodeLink)";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ClassName", model.ClassName);
                        cmd.Parameters.AddWithValue("@Status", model.Status);
                        cmd.Parameters.AddWithValue("@QRCodeLink", model.QRCodeLink ?? (object)DBNull.Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            return View(model);
        }


        // 🔷 EDIT GET
        public IActionResult Edit(int id)
        {
            Classroom model = new Classroom();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Classroom WHERE ClassId = @ClassId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClassId", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.ClassId = Convert.ToInt32(reader["ClassId"]);
                    model.ClassName = reader["ClassName"]?.ToString() ?? "";
                    model.Status = reader["Status"]?.ToString() ?? "";
                    model.QRCodeLink = reader["QRCodeLink"]?.ToString() ?? "";
                }
            }

            return View(model);
        }

        // 🔷 EDIT POST
        [HttpPost]
        public IActionResult Edit(Classroom model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Classroom SET ClassName = @ClassName, Status = @Status, QRCodeLink = @QRCodeLink WHERE ClassId = @ClassId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ClassId", model.ClassId);
                    cmd.Parameters.AddWithValue("@ClassName", model.ClassName);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@QRCodeLink", model.QRCodeLink ?? (object)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // 🔷 DELETE GET
        public IActionResult Delete(int id)
        {
            Classroom model = new Classroom();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Classroom WHERE ClassId = @ClassId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClassId", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.ClassId = Convert.ToInt32(reader["ClassId"]);
                    model.ClassName = reader["ClassName"]?.ToString() ?? "";
                    model.Status = reader["Status"]?.ToString() ?? "";
                    model.QRCodeLink = reader["QRCodeLink"]?.ToString() ?? "";
                }
            }

            return View(model);
        }

        // 🔷 DELETE POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Classroom WHERE ClassId = @ClassId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClassId", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // 🔷 QR Code Generator Action
        public IActionResult GenerateQRCode(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText ?? "", QRCodeGenerator.ECCLevel.Q);

            using (BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData))
            {
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                return File(qrCodeBytes, "image/png");
            }
        }

    }

}
