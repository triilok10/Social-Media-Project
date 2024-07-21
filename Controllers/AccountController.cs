using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Social_Media_Project.Models;
using System.Text;

namespace Social_Media_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        IHttpContextAccessor _httpContextAccessor;
        private readonly dynamic baseUrl;

        public AccountController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var request = _httpContextAccessor.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host.Value}/"; _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, HttpPost]
        public async Task<IActionResult> Signup(Account pAccount)
        {
            try
            {

                string Message = "";
                if (string.IsNullOrWhiteSpace(pAccount.Username) || string.IsNullOrWhiteSpace(pAccount.Fullname) || string.IsNullOrWhiteSpace(pAccount.Password) || string.IsNullOrWhiteSpace(pAccount.Phone) ||
                    string.IsNullOrWhiteSpace(pAccount.ConfirmPassword) || string.IsNullOrWhiteSpace(pAccount.Email))
                {
                    Message = "Please fill the required details.";
                    TempData["errorMessage"] = Message;
                    TempData.Keep("errorMessage");
                    return View();
                }
                else
                {
                    string url = baseUrl + "api/AccountAPI/Signup";
                    string jsonBody = JsonConvert.SerializeObject(pAccount);
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        string resData = JsonConvert.SerializeObject(res);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
        public IActionResult ForgottenPassword()
        {
            return View();
        }

    }
}
