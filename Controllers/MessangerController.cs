using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Social_Media_Project.AppCode;
using Social_Media_Project.Models;
using System.Text;

namespace Social_Media_Project.Controllers
{
    public class MessangerController : Controller
    {

        private readonly HttpClient _httpClient;
        IHttpContextAccessor _httpContextAccessor;
        private readonly dynamic baseUrl;

        private readonly ISessionService _sessionService;
        public MessangerController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            var request = _httpContextAccessor.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host.Value}/"; _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ChatMessanger(string Id = "")
        {
            bool response = false;
            string Message = "";

            var Username = _sessionService.GetString("Username");
            var UserId = _sessionService.GetInt32("UserId");
            try
            {
                if (Username != null && UserId != null)
                {
                    if (!string.IsNullOrWhiteSpace(Id))
                    {
                        string url = baseUrl + $"api/FirebaseMessagingAPI/GetChatMessage?Id={Id}&UserId={UserId}";
                        HttpResponseMessage res = await _httpClient.GetAsync(url);
                        if (res.IsSuccessStatusCode)
                        {
                            string urll = baseUrl + $"api/FirebaseMessagingAPI/GetChatMessageToken?UserId={UserId}";

                            HttpResponseMessage responsee = await _httpClient.GetAsync(urll);
                            if (responsee.IsSuccessStatusCode)
                            {
                                string fcmBody = await responsee.Content.ReadAsStringAsync();
                                MediaPost objMediaPost = JsonConvert.DeserializeObject<MediaPost>(fcmBody);

                                var fcmToken = objMediaPost.UserFcmToken;
                                _sessionService.SetString("UserFCMToken", fcmToken);
                                string resBody = await res.Content.ReadAsStringAsync();
                                MediaPost obj = JsonConvert.DeserializeObject<MediaPost>(resBody);

                                var messageFCMToken = obj.FCMToken;
                                _sessionService.SetString("MessageFCMToken", messageFCMToken);

                                return View(obj);
                            }
                            else
                            {
                                return BadRequest("Error in Fetching the Data from the API");
                            }
                        }
                        else
                        {
                            return BadRequest("Error in Fetching the Data from the API");
                        }



                    }
                    else
                    {
                        Message = "Please pass the require Parameter!";
                        response = false;
                        return Ok(new { Message, response });
                    }


                }
                else
                {

                    TempData["errorMessage"] = "Please Login";
                    TempData.Keep("errorMessage");
                    return View("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return BadRequest(Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] NotificationSend objNotification)
        {

            var UserToken = _sessionService.GetString("MessageFCMToken");


            string Message = "";
            bool response = false;


            try
            {
                if (UserToken != null)
                {
                    string url = baseUrl + "api/FirebaseMessagingAPI/SendMessage";
                    objNotification.Token = UserToken;
                    var JsonData = new
                    {
                        objNotification.Token,
                        objNotification.MessageContent
                    };
                    string JsonDataa = JsonConvert.SerializeObject(JsonData);
                    StringContent content = new StringContent(JsonDataa, Encoding.UTF8, "application/json");

                    HttpResponseMessage fcmResponse = await _httpClient.PostAsync(url, content);
                    if (fcmResponse.IsSuccessStatusCode)
                    {
                        string resBody = await fcmResponse.Content.ReadAsStringAsync();
                        return Json(new { success = true, message = "Message sent successfully" });
                    }
                    else
                    {
                        return StatusCode((int)fcmResponse.StatusCode, new { error = "Failed to send message via FCM" });
                    }

                }
                else
                {
                    return BadRequest("FCM Token is Missing here");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }



    }
}
