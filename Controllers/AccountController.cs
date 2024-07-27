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
                    string Json = JsonConvert.SerializeObject(pAccount);
                    StringContent content = new StringContent(Json, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        string resData = JsonConvert.DeserializeObject<string>(resBody);
                        return RedirectToAction("UserAccountPage");
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
                            var userId = new
                            {
                                pAccount.Id
                            };
                            pAccount.Id = Convert.ToInt32(resData.id);
                            TempData["successMessage"] = Message;
                            TempData.Keep("successMessage");
                            return RedirectToAction("UserAccountPage", userId);
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
                //string url = baseUrl + "api/AccountAPI/GetUserHomeDetails";
                //string fullUrl = $"{url}?Id={Id}";
                //HttpResponseMessage res = await _httpClient.GetAsync(fullUrl);
                //if (res.IsSuccessStatusCode)
                //{
                //    string resBody = await res.Content.ReadAsStringAsync();
                //    List<MediaPost> lstData = JsonConvert.DeserializeObject<List<MediaPost>>(resBody);
                //    return View(lstData);
                //}
                //else
                //{
                //    return BadRequest("Error fetching data from the API.");
                //}
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new { Message = ex.Message });
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
                        var pPostObj = new
                        {
                            pPost.PostCaption,
                            pPost.PhotoPath
                        };

                        string url = baseUrl + "api/AccountAPI/CreatePost";
                        string JsonBody = JsonConvert.SerializeObject(pPostObj);
                        StringContent content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            string resData = JsonConvert.DeserializeObject<string>(resBody);
                        }
                        else
                        {
                            return BadRequest("Error in Fetching Data or to Call the API.");
                        }
                        return View();
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
                    Message = "Please pass the required data.";
                    TempData["errorMessage"] = Message;
                    TempData.Keep("errorMessage");
                    return View();
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
