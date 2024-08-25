using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.Arm;

namespace Social_Media_Project.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly string _connectionString;

        public GoogleAuthController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CustomConnection");
        }

        [HttpPost]
        public IActionResult GoogleRegister(GoogleAuth googleAuth)
        {
            string Message = "";
            string UserId = "";
            string Username = "";
            try
            {
                string Name = googleAuth.Name.Replace("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name:", "").Trim();
                string Email = googleAuth.Email.Replace("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress:", "").Trim();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_GoogleLogin", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Mode", 1);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Mobile", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProfilePhotoPath", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProfileBio", DBNull.Value);

                    SqlParameter message = new SqlParameter("@Message", SqlDbType.VarChar, 200);
                    message.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(message);

                    SqlParameter userId = new SqlParameter("@UserId", SqlDbType.Int);
                    userId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(userId);

                    SqlParameter UsernameLogin = new SqlParameter("@UsernameLogin", SqlDbType.VarChar, 50);
                    UsernameLogin.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(UsernameLogin);

                    cmd.ExecuteNonQuery();
                    Message = message.Value.ToString();
                    UserId = userId.Value.ToString();
                    Username = UsernameLogin.Value.ToString();
                }
                return Ok(new { msg = Message, id = UserId, username = Username });
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new
                {
                    errmsg = Message
                });
            }
        }

        [HttpGet]
        public IActionResult GetListUpdate(int id = 0)
        {
            string Message = "";
            try
            {
                Account obj = new Account();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_GoogleLogin", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    con.Open();
                    cmd.Parameters.AddWithValue("@Mode", 3);
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlParameter message = new SqlParameter("@Message", SqlDbType.VarChar, 200);
                    message.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(message);

                    SqlParameter userId = new SqlParameter("@UserId", SqlDbType.Int);
                    userId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(userId);

                    SqlParameter UsernameLogin = new SqlParameter("@UsernameLogin", SqlDbType.VarChar, 50);
                    UsernameLogin.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(UsernameLogin);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Fullname = Convert.ToString(reader["FullName"]);
                            obj.Email = Convert.ToString(reader["Email"]);
                        }
                    }
                }
                return Ok(obj);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
        }

        [HttpPost]
        public IActionResult PostListUpdate([FromBody] Account pAccount)
        {
            string Message = "";
            try
            {
                if (pAccount.hdnId != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("usp_GoogleLogin", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.AddWithValue("@Mode", 2);
                        cmd.Parameters.AddWithValue("@Id", pAccount.Id);
                        cmd.Parameters.AddWithValue("@Name", pAccount.Fullname);
                        cmd.Parameters.AddWithValue("@Mobile", pAccount.Phone);
                        cmd.Parameters.AddWithValue("@Username", pAccount.Username);
                        cmd.Parameters.AddWithValue("@ProfilePhotoPath", pAccount.ProfilePhotoPath);
                        cmd.Parameters.AddWithValue("@ProfileBio", pAccount.Bio);
                        cmd.Parameters.AddWithValue("@DateOfBirth", pAccount.DateOfBirth);
                        SqlParameter message = new SqlParameter("@Message", SqlDbType.VarChar, 200);
                        message.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(message);

                        SqlParameter UsernameLogin = new SqlParameter("@UsernameLogin", SqlDbType.VarChar, 50);
                        UsernameLogin.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(UsernameLogin);

                        SqlParameter userId = new SqlParameter("@UserId", SqlDbType.Int);
                        userId.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(userId);
                        cmd.ExecuteNonQuery();
                        Message = message.Value.ToString();
                    }
                    Message = "Data Updated Successfully";
                    return Ok(new { msg = Message });
                }
                else
                {
                    Message = "Please pass the required Parameter's.";
                    return Ok(new { errmsg = Message });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { errmsg = Message });
            }
        }
    }
}
