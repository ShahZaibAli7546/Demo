using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Demo.Models;
using System.Security.Cryptography;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var con = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM AdminLogin WHERE Email = @Email AND Password = @Password", con);

                cmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
                cmd.Parameters.AddWithValue("@Password", model.Password ?? string.Empty);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    TempData["UserEmail"] = model.Email;
                    return RedirectToAction("ControlPanel");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View(model);
        }

        // GET: Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string token = GenerateResetToken();
            var connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var con = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(
                    "UPDATE AdminLogin SET ResetToken = @Token, TokenExpiry = @Expiry WHERE Username = @Username OR Email = @Username", con);

                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@Expiry", DateTime.Now.AddMinutes(15));
                cmd.Parameters.AddWithValue("@Username", model.Identifier ?? string.Empty);

                con.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
                    TempData["Message"] = $"Reset your password by clicking <a href='{resetLink}'>here</a>.";
                    return RedirectToAction("ForgotPassword");
                }

                ModelState.AddModelError(string.Empty, "User not found.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View(model);
        }

        // GET: Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            ViewBag.Token = token;
            return View();
        }

        // POST: Reset Password
        [HttpPost]
        public IActionResult ResetPassword(string? token, string? newPassword, string? confirmPassword)
        {
            ViewBag.Token = token;

            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(string.Empty, "Reset token is missing.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Both password fields are required.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View();
            }

            var connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var con = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(
                    "UPDATE AdminLogin SET Password = @Password, ResetToken = NULL, TokenExpiry = NULL WHERE ResetToken = @Token AND TokenExpiry > GETDATE()", con);

                cmd.Parameters.AddWithValue("@Password", newPassword);
                cmd.Parameters.AddWithValue("@Token", token);

                con.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    TempData["Message"] = "Password reset successful. You can now log in.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View();
        }

        // GET: Control Panel
        [HttpGet]
        public IActionResult ControlPanel()
        {
            ViewBag.UserEmail = TempData["UserEmail"] ?? "admin@example.com";
            return View("ControlPanel");
        }
        // GET: Account/Business
        [HttpGet]
        public IActionResult Business()
        {
            return View(); // Looks for Views/Account/Business.cshtml
        }

        // GET: Settings
        [HttpGet("Account/ControlPanel/Settings")]
        public IActionResult Settings()
        {
            ViewBag.UserEmail = TempData["UserEmail"] ?? "admin@example.com";
            return View("Settings");
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Login");
        }

        // Utility: Generate secure token
        private static string GenerateResetToken()
        {
            byte[] tokenBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
