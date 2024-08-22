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
                            string resBody = await res.Content.ReadAsStringAsync();
                            MediaPost obj = JsonConvert.DeserializeObject<MediaPost>(resBody);
                            return View(obj);
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


    }
}
