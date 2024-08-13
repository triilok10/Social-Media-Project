using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Social_Media_Project.Controllers.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FirebaseMessagingAPI : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FirebaseMessagingAPI(IConfiguration configuration)
        {
            _configuration = configuration;

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(_configuration["Firebase:ServiceAccountPath"])
                });
            }
        }

        [HttpPost("send")]
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
    }
}
