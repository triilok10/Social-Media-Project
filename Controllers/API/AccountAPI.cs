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
        public async Task<IActionResult> Signup([FromBody] Account pAccount)
        {
            string Message = "";
            try
            {
                if (pAccount == null)
                {
                    Message = "Please fill the required inputs.";
                }

                if (pAccount.Password != pAccount.ConfirmPassword)
                {
                    Message = "Password and Confirm Password doesn't match!";
                    return Ok(new { msg = Message });
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    SqlCommand cmd = new SqlCommand("usp_SignUp", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mobile", pAccount.Phone);
                    cmd.Parameters.AddWithValue("@Email", pAccount.Email);
                    cmd.Parameters.AddWithValue("@Fullname", pAccount.Fullname);
                    cmd.Parameters.AddWithValue("@NewPassword", pAccount.Password);
                    cmd.Parameters.AddWithValue("@ConfirmPassword", pAccount.ConfirmPassword);
                    cmd.Parameters.AddWithValue("@Username", pAccount.Username);
                    cmd.ExecuteNonQueryAsync();
                }
                Message = $"{pAccount.Fullname} account created!";
            }
            catch (SqlException ex)
            {
                Message = ex.Message;
                return BadRequest(new { msg = Message });
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Ok(new { msg = Message });
        }
    }
}
