using Microsoft.Data.SqlClient; // ✅ Required for SqlConnection and SqlCommand
using System.Data;              // ✅ Optional: used for Data-related operations (e.g., CommandType)
using Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class AccountsDisplayController(IConfiguration configuration) : Controller
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        public IActionResult Index()
        {
            var result = new List<AccountGroupWithAccountsViewModel>();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            var groupCmd = new SqlCommand("SELECT * FROM AccountGroups WHERE IsActive = 1", con);
            using var groupReader = groupCmd.ExecuteReader();
            var groups = new List<AccountGroup>();

            while (groupReader.Read())
            {
                groups.Add(new AccountGroup
                {
                    Id = Convert.ToInt32(groupReader["Id"]),
                    GroupCode = groupReader["GroupCode"].ToString()!,
                    GroupName = groupReader["GroupName"].ToString()!,
                    StatementType = groupReader["StatementType"].ToString()!,
                    IsActive = Convert.ToBoolean(groupReader["IsActive"])
                });
            }

            groupReader.Close();

            foreach (var group in groups)
            {
                var accounts = new List<Account>();

                var accCmd = new SqlCommand("SELECT * FROM Accounts WHERE GroupId = @GroupId", con);
                accCmd.Parameters.AddWithValue("@GroupId", group.Id);
                using var accReader = accCmd.ExecuteReader();
                while (accReader.Read())
                {
                    accounts.Add(new Account
                    {
                        Id = Convert.ToInt32(accReader["Id"]),
                        AccountCode = accReader["AccountCode"].ToString()!,
                        Title = accReader["Title"].ToString()!,
                        IsActive = Convert.ToBoolean(accReader["IsActive"]),
                        GroupId = Convert.ToInt32(accReader["GroupId"])
                    });
                }

                accReader.Close();

                result.Add(new AccountGroupWithAccountsViewModel
                {
                    GroupId = group.Id,
                    GroupCode = group.GroupCode,
                    GroupName = group.GroupName,
                    StatementType = group.StatementType,
                    IsActive = group.IsActive,
                    Accounts = accounts
                });
            }

            return View(result);
        }
    }

}
