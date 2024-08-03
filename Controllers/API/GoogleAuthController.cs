using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;
using System.Data.SqlClient;

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
                    cmd.ExecuteNonQuery();
                }
                Message = "Login successfully, Please update your profile.";
                return Ok(new { msg = Message });
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
    }
}
