using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;
using System.Data;
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

        #region "SignUp Get/Post"

        [HttpPost]
        public IActionResult Signup([FromBody] Account pAccount)
        {
            string Message = "";
            string AccountId = "";
            try
            {
                if (pAccount.Id == 0)
                {
                    if (pAccount == null)
                    {
                        Message = "Please fill the required inputs.";
                        return Ok(new { msg = Message });
                    }

                    if (pAccount.Password != pAccount.ConfirmPassword)
                    {
                        Message = "Password and Confirm-Password didn't match!";
                        return Ok(new { msg = Message });
                    }

                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand("usp_SignUp", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Mode", 1);
                        cmd.Parameters.AddWithValue("@Mobile", pAccount.Phone);
                        cmd.Parameters.AddWithValue("@Email", pAccount.Email);
                        cmd.Parameters.AddWithValue("@Fullname", pAccount.Fullname);
                        cmd.Parameters.AddWithValue("@NewPassword", pAccount.Password);
                        cmd.Parameters.AddWithValue("@ConfirmPassword", pAccount.ConfirmPassword);
                        cmd.Parameters.AddWithValue("@Username", pAccount.Username);

                        SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 200);
                        outputMessage.Direction = ParameterDirection.Output;
                        SqlParameter signupId = new SqlParameter("@AccountId", SqlDbType.NVarChar, 20);
                        signupId.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(outputMessage);
                        cmd.Parameters.Add(signupId);
                        cmd.ExecuteNonQuery();
                        Message = cmd.Parameters["@Message"].Value.ToString();
                        AccountId = cmd.Parameters["@AccountId"].Value.ToString();
                    }

                    return Ok(new { msg = Message, id = AccountId });
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_SignUp", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Mode", 2);
                        cmd.Parameters.AddWithValue("@Id", pAccount.Id);
                        cmd.Parameters.AddWithValue("@ProfilePhotoPath", pAccount.ProfilePhotoPath);
                        cmd.Parameters.AddWithValue("@Bio", pAccount.Bio);
                        cmd.Parameters.AddWithValue("@DateOfBirth", pAccount.DateOfBirth);
                        SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 200);
                        outputMessage.Direction = ParameterDirection.Output;
                        SqlParameter signupId = new SqlParameter("@AccountId", SqlDbType.NVarChar, 20);
                        signupId.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputMessage);
                        cmd.Parameters.Add(signupId);
                        cmd.ExecuteNonQuery();
                        Message = cmd.Parameters["@Message"].Value.ToString();
                    }
                    return Ok(new { msg = Message });
                }

            }
            catch (SqlException ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
        }

        #endregion

    }
}
