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


        public class ChatMessageRequest
        {
            public string RecipientUserId { get; set; }
            public string MessageContent { get; set; }
        }

        #region "GetChatMessage"
        [HttpGet]
        public IActionResult GetChatMessage(string Id = "", string UserId = "")
        {
            string Message = "";
            bool response = false;
            try
            {
                if (UserId != null && Id != null)
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
                                objAccount.FCMToken = Convert.ToString(rdr["FCMToken"]);
                            }
                            response = true;
                        }
                    }
                    return Ok(objAccount);
                }
                else
                {
                    Message = "Please pass the required Parameter!";
                    response = false;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                response = false;
            }
            return Ok(new { Message, response });
        }

        #endregion

        #region "Get Chat Message Token"
        [HttpGet]
        public IActionResult GetChatMessageToken(string UserId = "")
        {
            string Message = "";
            bool response = false;
            try
            {
                if (UserId != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_GetUserDetails", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Mode", 3);
                        cmd.Parameters.AddWithValue("@Id", UserId);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                MediaPost post = new MediaPost
                                {
                                    UserFcmToken = Convert.ToString(rdr["FCMToken"])
                                };
                                return Ok(post);
                            }
                        }
                    }
                }
                else
                {
                    Message = "Please pass the required Parameter";
                    response = false;
                }
            }
            catch (Exception ex)
            {
                response = false;
                Message = ex.Message;
            }
            return Ok(new { msg = Message, res = response });
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] NotificationSend notification)
        {
            if (string.IsNullOrEmpty(notification.Token) || string.IsNullOrEmpty(notification.MessageContent))
            {
                return BadRequest(new { Error = "Invalid request data" });
            }

            try
            {
                var message = new Message
                {
                    Token = notification.Token,
                    Notification = new Notification
                    {
                        Title = "New Message",
                        Body = notification.MessageContent
                    }
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return Ok(new { Response = response, Status = "Message sent successfully" });
            }
            catch (FirebaseMessagingException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


    }
}
