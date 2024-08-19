using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Social_Media_Project.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Social_Media_Project.Controllers.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FirebaseMessagingAPI : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;





        public IActionResult Test()
        {
            string Message = "Test Ok";
            return Ok(new { msg = Message });
        }
        public FirebaseMessagingAPI(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("CustomConnection");
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(_configuration["Firebase:ServiceAccountPath"])
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (string.IsNullOrEmpty(request.RecipientUserId) || string.IsNullOrEmpty(request.MessageContent))
            {
                return BadRequest("Invalid request.");
            }

            var message = new Message()
            {
                Token = request.RecipientUserId,
                Notification = new Notification()
                {
                    Title = "New Message",
                    Body = request.MessageContent
                }
            };

            string response;
            try
            {
                response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return Ok(new { MessageId = response });
            }
            catch (FirebaseMessagingException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        public class ChatMessageRequest
        {
            public string RecipientUserId { get; set; }
            public string MessageContent { get; set; }
        }

        #region "GetChatMessage"
        [HttpGet]
        public IActionResult GetChatMessage(string Id = "")
        {
            string Message = "";
            bool response = false;
            try
            {
                MediaPost objAccount = new MediaPost();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_GetUserDetails", con);
                    con.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 1);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            objAccount.Fullname = Convert.ToString(rdr["FullName"]);
                            objAccount.ProfilePhotoPath = Convert.ToString(rdr["ProfilePhotoPath"]);
                            objAccount.Bio = Convert.ToString(rdr["ProfileBio"]);
                            objAccount.DateOfBirth = Convert.ToDateTime(rdr["DateOfBirth"]);
                            objAccount.Username = Convert.ToString(rdr["Username"]);
                            objAccount.Id = Convert.ToInt32(rdr["Id"]);
                        }
                        response = true;
                    }
                }
                return Ok(objAccount);
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        #endregion


    }
}
