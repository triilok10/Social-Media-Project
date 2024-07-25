using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Social_Media_Project.Models;
using System.Drawing;
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

        #region "SignUp Get/Post"

        [HttpGet, HttpPost]
        public async Task<IActionResult> Signup(Account pAccount, IFormFile ProfilePhoto)
        {
            string Message = "";
            string Id = "";
            try
            {
                if (pAccount.hdnId == 1)
                {
                    if (string.IsNullOrWhiteSpace(pAccount.Username) || string.IsNullOrWhiteSpace(pAccount.Fullname) || string.IsNullOrWhiteSpace(pAccount.Password) || string.IsNullOrWhiteSpace(pAccount.Phone) ||
                        string.IsNullOrWhiteSpace(pAccount.ConfirmPassword) || string.IsNullOrWhiteSpace(pAccount.Email))
                    {
                        Message = "Please fill the required details.";
                        TempData["errorMessage"] = Message;
                        TempData.Keep("errorMessage");
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
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                            Message = resData.msg;
                            Id = resData.id;
                            if (Message == "Password and Confirm-Password didn't match!")
                            {
                                TempData["errorMessage"] = Message;
                                TempData.Keep("errorMessage");
                            }
                            else if (Message == "Username already exists.")
                            {
                                TempData["errorMessage"] = Message;
                                TempData.Keep("errorMessage");
                            }
                            else if (Message == "Email already exists.")
                            {
                                TempData["errorMessage"] = Message;
                                TempData.Keep("errorMessage");
                            }
                            else
                            {
                                pAccount.Id = Convert.ToInt32(Id);
                                TempData["successMessage"] = Message;
                                TempData.Keep("successMessage");
                                return View(pAccount);
                            }
                        }
                    }
                    return View();
                }
                else if (!string.IsNullOrWhiteSpace(pAccount.Id.ToString()))
                {
                    if (ProfilePhoto != null && ProfilePhoto.Length > 0)
                    {
                        string FileName = Path.GetFileName(ProfilePhoto.FileName);
                        string FileData = Path.Combine("wwwroot", "images", "UserData");
                        if (!Directory.Exists(FileData))
                        {
                            Directory.CreateDirectory(FileData);
                        }
                        string FilePath = Path.Combine(FileData, FileName);
                        using (var Stream = new FileStream(FilePath, FileMode.Create))
                        {
                            await ProfilePhoto.CopyToAsync(Stream);
                        }

                        pAccount.ProfilePhotoPath = "/images/UserData/" + FileName;

                        string url = baseUrl + "api/AccountAPI/Signup";

                        var pAccountData = new
                        {
                            pAccount.ProfilePhotoPath,
                            pAccount.Id,
                            pAccount.Bio,
                            pAccount.DateOfBirth
                        };

                        string jsonBody = JsonConvert.SerializeObject(pAccountData);
                        StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                            Message = resData.msg;
                            pAccount.Id = Convert.ToInt32(resData.id);
                            TempData["successMessage"] = Message;
                            TempData.Keep("successMessage");
                            return RedirectToAction("UserAccountPage", pAccount);
                        }
                    }
                    return View();
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        #endregion

        public IActionResult ForgottenPassword()
        {
            return View();
        }


        #region "User Account Page"
        public async Task<IActionResult> UserAccountPage(Account pAccount)
        {
            string Message = "";
            int Id = pAccount.Id;
            try
            {
                string url = baseUrl + "api/AccountAPI/GetUserHomeDetails";
                string fullUrl = $"{url}?Id={Id}";
                HttpResponseMessage res = await _httpClient.GetAsync(fullUrl);
                if (res.IsSuccessStatusCode)
                {
                    string resBody = await res.Content.ReadAsStringAsync();
                    Account resData = JsonConvert.DeserializeObject<Account>(resBody);
                    return View(resData);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return View(Message);
            }
        }
        #endregion
    }
}
