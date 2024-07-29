using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Social_Media_Project.AppCode;
using Social_Media_Project.Models;
using System.Drawing;
using System.Security.Principal;
using System.Text;

namespace Social_Media_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        IHttpContextAccessor _httpContextAccessor;
        private readonly dynamic baseUrl;

        private readonly ISessionService _sessionService;
        public AccountController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
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

        #region "Login Post"
        [HttpPost]
        public async Task<IActionResult> Login(Account pAccount)
        {
            string Message = "";
            try
            {
                if (pAccount.Username != null && pAccount.Password != null)
                {
                    string url = baseUrl + "api/AccountAPI/LoginVerify";
                    var pAccountData = new
                    {
                        pAccount.Username,
                        pAccount.Password
                    };
                    string Json = JsonConvert.SerializeObject(pAccountData);
                    StringContent content = new StringContent(Json, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);

                        string UserId = resData.id;
                        string errorMessage = resData.errmsg;

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            TempData["errorMessage"] = errorMessage;
                            TempData.Keep("errorMessage");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            _sessionService.SetString("Username", pAccount.Username);
                            _sessionService.SetInt32("UserId", Convert.ToInt32(UserId));

                            return RedirectToAction("UserAccountPage");
                        }
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
                else
                {
                    Message = "Please enter the UserName and Password";
                    TempData["errorMessage"] = Message;
                    TempData.Keep("errorMessage");
                    return View("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return Ok(new { Message = ex.Message });
            }
        }
        #endregion

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
                                _sessionService.SetString("Username", pAccount.Username);
                                _sessionService.SetInt32("UserId", pAccount.Id);
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
                    if (_sessionService.GetString("Username") != null && _sessionService.GetInt32("UserId") != null)
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
                                TempData["successMessage"] = Message;
                                TempData.Keep("successMessage");
                                return RedirectToAction("UserAccountPage");
                            }
                        }
                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View("Index", "Home");
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
            try
            {
                string currentActionUrl = HttpContext.Request.Path;
                TempData["currentActionUrl"] = currentActionUrl;
                TempData.Keep("currentActionUrl");
                var username = _sessionService.GetString("Username");
                var userId = _sessionService.GetInt32("UserId");
                if (username != null && userId != null)
                {
                    //Call API to Get the Bio Details
                    string url = baseUrl + "api/AccountAPI/GetUserBioDetails";
                    string fullUrl = $"{url}?Id={userId}";
                    HttpResponseMessage res = await _httpClient.GetAsync(fullUrl);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                        ViewBag.fullname = resData.fullname;
                        ViewBag.profilePhotoPath = resData.profilePhotoPath;
                        ViewBag.bio = resData.bio;
                        ViewBag.dateOfBirth = resData.dateOfBirth;

                        //Call the API to get the User Post Details here.
                        string purl = baseUrl + "api/AccountAPI/GetUserPostDetails";
                        string pfullUrl = $"{purl}?Id={userId}";
                        HttpResponseMessage pres = await _httpClient.GetAsync(pfullUrl);
                        if (pres.IsSuccessStatusCode)
                        {
                            string presBody = await pres.Content.ReadAsStringAsync();
                            List<MediaPost> lstData = JsonConvert.DeserializeObject<List<MediaPost>>(presBody);
                            return View(lstData);
                        }
                        else
                        {
                            return BadRequest("Error fetching data from the API.");
                        }
                    }
                    else
                    {
                        return BadRequest("Error fetching data from the API.");
                    }

                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return View("Error", new { msg = Message });
            }
        }
        #endregion

        #region "Add Post"
        [HttpGet, HttpPost]
        public async Task<IActionResult> AddPost(MediaPost pPost, IFormFile AddPostPhoto)
        {
            string Message = "";
            try
            {
                var username = _sessionService.GetString("Username");
                var userId = _sessionService.GetInt32("UserId");
                if (username != null && userId != null)
                {
                    if (pPost.hdnId == 1)
                    {
                        if (pPost.AddPostPhoto != null)
                        {
                            string FileName = Path.GetFileName(AddPostPhoto.FileName);
                            string FileData = Path.Combine("wwwroot", "images", "UserPost");
                            if (!Directory.Exists(FileData))
                            {
                                Directory.CreateDirectory(FileData);
                            }
                            string FilePath = Path.Combine(FileData, FileName);
                            using (var Stream = new FileStream(FilePath, FileMode.Create))
                            {
                                await AddPostPhoto.CopyToAsync(Stream);
                            }

                            pPost.PhotoPath = FilePath;
                            pPost.Id = userId;
                            var pPostObj = new
                            {
                                pPost.PostCaption,
                                pPost.PhotoPath,
                                pPost.Id
                            };

                            string url = baseUrl + "api/AccountAPI/CreatePost";
                            string JsonBody = JsonConvert.SerializeObject(pPostObj);
                            StringContent content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                            HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                            if (res.IsSuccessStatusCode)
                            {
                                string resBody = await res.Content.ReadAsStringAsync();
                                dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                                Message = resData.msg;
                                TempData["successMessage"] = Message;
                                TempData.Keep("successMessage");
                                return RedirectToAction("UserAccountPage");
                            }
                            else
                            {
                                return BadRequest("Error in Fetching Data or to Call the API.");
                            }

                        }
                        else
                        {
                            Message = "Please pass the required data.";
                            TempData["errorMessage"] = Message;
                            TempData.Keep("errorMessage");
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return View("Error", new { Message = ex.Message });
            }
        }
        #endregion
    }
}
