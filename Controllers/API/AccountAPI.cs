using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;
using System.Data.SqlClient;

namespace Social_Media_Project.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountAPI : ControllerBase
    {
        private readonly string _connectionString;

        public AccountAPI(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CustomConnection");
        }

        [HttpPost]
        public IActionResult Signup([FromBody] Account pAccount)
        {
            string Message = "";
            try
            {
                if (pAccount == null)
                {
                    Message = "Please fill the required inputs.";
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_SignUp", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@phone", pAccount.Phone);
                    cmd.Parameters.AddWithValue("@email", pAccount.Email);
                    cmd.Parameters.AddWithValue("@FullName", pAccount.Fullname);
                    cmd.Parameters.AddWithValue("@password", pAccount.Password);
                    cmd.Parameters.AddWithValue("@confirmpassword", pAccount.ConfirmPassword);
                    cmd.Parameters.AddWithValue("@username", pAccount.Username);
                    cmd.Parameters.AddWithValue("@phone", pAccount.Phone);
                    cmd.BeginExecuteNonQuery();
                }
                Message = $"{pAccount.Fullname} account created!";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Ok(new { msg = Message });
        }
    }
}
