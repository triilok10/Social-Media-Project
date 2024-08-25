using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Social_Media_Project.AppCode;
using Social_Media_Project.Models;
using System.Text;
using System.Security.Claims;
using System.Text.Unicode;

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

        #region "Logout
        public IActionResult Logout()
        {
            string Message = "";
            try
            {
                _sessionService.Remove("Username");
                _sessionService.Remove("UserId");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                TempData["errorMessage"] = "Error in Clearing the Session";
                TempData.Keep("errorMessage");
                return RedirectToAction("Index", "Home");
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
                        ViewBag.username = resData.username;

                        //Call the API to get the User Post Details here.
                        string purl = baseUrl + "api/AccountAPI/GetUserPostDetails";
                        string pfullUrl = $"{purl}?Id={userId}";
                        HttpResponseMessage pres = await _httpClient.GetAsync(pfullUrl);
                        if (pres.IsSuccessStatusCode)
                        {
                            string presBody = await pres.Content.ReadAsStringAsync();
                            List<MediaPost> lstData = JsonConvert.DeserializeObject<List<MediaPost>>(presBody);

                            //Call the API to Get the User Following and Follower

                            string FollowUrl = baseUrl + $"api/AccountAPI/GetFollowingData?Id={userId}";
                            HttpResponseMessage followRes = await _httpClient.GetAsync(FollowUrl);
                            if (followRes.IsSuccessStatusCode)
                            {
                                string followBody = await followRes.Content.ReadAsStringAsync();
                                MediaPost obj = JsonConvert.DeserializeObject<MediaPost>(followBody);
                                ViewBag.Follower = obj.Follower;
                                ViewBag.Following = obj.Following;
                                ViewBag.PostCount = obj.PostCount;
                            }
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
        public async Task<IActionResult> AddPost(MediaPost pPost, IFormFile AddPostPhoto, int PostId = 0)
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
                    if (pPost.hdnId == 1 &&  AddPostPhoto !=null && pPost.Id ==null)
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

                            pPost.PhotoPath = "/images/UserPost/" + FileName;
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
                    else if (PostId != null && pPost.Id == null && PostId > 0)
                    {
                        string EditURL = baseUrl + $"api/AccountAPI/UpdatePost?PostId={PostId}";

                        HttpResponseMessage editRes = await _httpClient.GetAsync(EditURL);
                        if (editRes.IsSuccessStatusCode)
                        {
                            string editBody = await editRes.Content.ReadAsStringAsync();

                            MediaPost obj = JsonConvert.DeserializeObject<MediaPost>(editBody);
                            ViewBag.PhotoPath = obj.PhotoPath;

                            return View(obj);
                        }
                        else
                        {
                            return BadRequest("Data didnt fetched!");
                        }

                    }
                    else if (PostId == 0 && pPost.Id > 0)
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
                            pPost.PhotoPath = "/images/UserPost/" + FileName;
                        }
                        else
                        {
                            pPost.PhotoPath = pPost.HdnPostPhotoPath;


                        }

                        string UpdatePost = baseUrl + "api/AccountAPI/UpdatePostSection";



                        var JsonData = new
                        {
                            pPost.Id,
                            pPost.PhotoPath,
                            pPost.PostCaption
                        };

                        string con = JsonConvert.SerializeObject(JsonData);
                        StringContent Content = new StringContent(con, Encoding.UTF8, "application/json");
                        HttpResponseMessage res = await _httpClient.PostAsync(UpdatePost, Content);
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
                            return BadRequest("Unable to update the Post");
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

        #region "Search Account"
        [HttpGet, HttpPost]
        public async Task<IActionResult> SearchProfile(string SearchName = "", string hidden = "")
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
                    if (hidden == "1")
                    {
                        string url = baseUrl + "api/AccountAPI/SearchProfile";
                        string fullUrl = url + $"?SearchName={SearchName}";

                        HttpResponseMessage res = await _httpClient.GetAsync(fullUrl);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            List<MediaPost> lstMedia = JsonConvert.DeserializeObject<List<MediaPost>>(resBody);
                            return Ok(lstMedia);
                        }
                        else
                        {
                            return BadRequest("Error to fetch the Data");
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
                Message = ex.Message;
                TempData["errorMessage"] = Message;
                TempData.Keep("errorMessage");
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion


        #region "Feed "
        public async Task<IActionResult> Feed()
        {
            string Message = "";
            try
            {
                var username = _sessionService.GetString("Username");
                var userId = _sessionService.GetInt32("UserId");
                if (username != null && userId != null)
                {
                    string url = baseUrl + "api/AccountAPI/GetUserBioDetails";
                    string fullUrl = $"{url}?Id={userId}";
                    HttpResponseMessage res = await _httpClient.GetAsync(fullUrl);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);

                        ViewBag.profilePhotoPath = resData.profilePhotoPath;

                        ViewBag.username = resData.username;
                        string urll = baseUrl + "api/AccountAPI/FeedData";
                        HttpResponseMessage ress = await _httpClient.GetAsync(urll);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBodyy = await ress.Content.ReadAsStringAsync();
                            List<MediaPost> lstPost = JsonConvert.DeserializeObject<List<MediaPost>>(resBodyy);
                            return View(lstPost);
                        }
                        else
                        {
                            Message = "Error in Fetching Data";
                            TempData["errorMessage"] = Message;
                            TempData.Keep("errorMessage");
                            return View();
                        }
                    }
                    else
                    {
                        return BadRequest("Error in Calling the API to Fetch the Data");
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
                TempData["errorMessage"] = Message;
                TempData.Keep("errorMessage");
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region "Google Auth Login/ Logout"
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { returnUrl }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "Account");
        }

        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims;
            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (!result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                GoogleAuth googleAuth = new GoogleAuth();
                googleAuth.Name = nameClaim.ToString();
                googleAuth.Email = emailClaim.ToString();

                string url = baseUrl + "api/GoogleAuth/GoogleRegister";
                string Jsonbody = JsonConvert.SerializeObject(googleAuth);
                StringContent content = new StringContent(Jsonbody, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                if (res.IsSuccessStatusCode)
                {
                    string resBody = await res.Content.ReadAsStringAsync();
                    dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                    string message = resData.msg.ToString();
                    int userId = resData.id;
                    if (resData.msg == "Email already exists.")
                    {
                        string username = resData.username;
                        _sessionService.SetInt32("UserId", userId);
                        _sessionService.SetString("Username", username);
                        return RedirectToAction("UserAccountPage", "Account");
                    }
                    else
                    {
                        TempData["successMessage"] = message;
                        TempData.Keep("successMessage");
                        _sessionService.SetInt32("UserId", userId);
                        string hdnData = "Login123";
                        _sessionService.SetString("hdnData", hdnData);
                        return RedirectToAction("UpdateGoogleForm", "Account");
                    }
                }
                else
                {
                    return Redirect(returnUrl);
                }


            }

        }
        #endregion

        #region "Google Response Update Form"
        [HttpGet, HttpPost]
        public async Task<IActionResult> UpdateGoogleForm(Account pAccount, IFormFile ProfilePhoto)
        {
            string Message = "";
            try
            {
                string currentActionUrl = HttpContext.Request.Path;
                TempData["currentActionUrl"] = currentActionUrl;
                TempData.Keep("currentActionUrl");
                var Id = _sessionService.GetInt32("UserId");
                var Data = _sessionService.GetString("hdnData");
                if (Id != null && Data != null)
                {
                    if (pAccount.hdnId == null)
                    {
                        string UserId = Convert.ToString(Id);
                        string url = baseUrl + $"api/GoogleAuth/GetListUpdate/?id={UserId}";
                        HttpResponseMessage res = await _httpClient.GetAsync(url);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            Account Obj = JsonConvert.DeserializeObject<Account>(resBody);
                            return View(Obj);
                        }
                        else
                        {
                            return BadRequest("Error in Fetching Data");
                        }
                    }
                    else
                    {
                        if (ProfilePhoto != null)
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

                        }
                        pAccount.Id = Convert.ToInt32(Id);

                        var pAccountData = new
                        {
                            pAccount.hdnId,
                            pAccount.Id,
                            pAccount.ProfilePhotoPath,
                            pAccount.Fullname,
                            pAccount.DateOfBirth,
                            pAccount.Phone,
                            pAccount.Bio,
                            pAccount.Username
                        };

                        string JsonObject = JsonConvert.SerializeObject(pAccountData);
                        StringContent content = new StringContent(JsonObject, Encoding.UTF8, "application/json");
                        string url = baseUrl + "api/GoogleAuth/PostListUpdate";
                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            _sessionService.SetString("Username", pAccount.Username);
                            return RedirectToAction("UserAccountPage", "Account");
                        }
                        else
                        {
                            return BadRequest("Error in Updating the Data");
                        }
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
                return Ok(Message);
            }
        }
        #endregion

        #region "Search Profile Homepage"
        [HttpGet]
        public async Task<IActionResult> SearchUserHomePage(string Id = "")
        {
            string Message = "";
            try
            {
                string currentActionUrl = HttpContext.Request.Path;
                TempData["currentActionUrl"] = currentActionUrl;
                TempData.Keep("currentActionUrl");
                var UserId = _sessionService.GetInt32("UserId");
                var Username = _sessionService.GetString("Username");
                if (UserId != null && Username != null)
                {
                    if (!string.IsNullOrWhiteSpace(Id))
                    {
                        string url = baseUrl + $"api/AccountAPI/GetUserBioDetails?Id={Id}";
                        HttpResponseMessage res = await _httpClient.GetAsync(url);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                            ViewBag.fullname = resData.fullname;
                            ViewBag.profilePhotoPath = resData.profilePhotoPath;
                            ViewBag.bio = resData.bio;
                            ViewBag.dateOfBirth = resData.dateOfBirth;
                            ViewBag.username = resData.username;
                            ViewBag.id = resData.id;
                            var CheckId = resData.id;

                            if (CheckId == UserId)
                            {
                                return RedirectToAction("UserAccountPage", "Account");
                            }
                            else
                            {
                                string purl = baseUrl + "api/AccountAPI/GetUserPostDetails";
                                string pfullUrl = $"{purl}?Id={Id}";
                                HttpResponseMessage pres = await _httpClient.GetAsync(pfullUrl);
                                if (pres.IsSuccessStatusCode)
                                {
                                    string presBody = await pres.Content.ReadAsStringAsync();
                                    List<MediaPost> lstData = JsonConvert.DeserializeObject<List<MediaPost>>(presBody);

                                    //Call the API here to get the Data of the dynamic Data of the No. of Follower's and Following
                                    string FollowUrl = baseUrl + $"api/AccountAPI/GetFollowingData?Id={Id}";
                                    HttpResponseMessage followRes = await _httpClient.GetAsync(FollowUrl);
                                    if (followRes.IsSuccessStatusCode)
                                    {
                                        string followBody = await followRes.Content.ReadAsStringAsync();
                                        MediaPost obj = JsonConvert.DeserializeObject<MediaPost>(followBody);
                                        ViewBag.Follower = obj.Follower;
                                        ViewBag.Following = obj.Following;
                                        ViewBag.PostCount = obj.PostCount;
                                    }

                                    return View(lstData);

                                }
                                else
                                {
                                    return BadRequest("Error fetching data from the API.");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("Error in Fetching Data");
                        }
                    }
                    else
                    {
                        Message = "Please pass the required Parameter!";
                        return Ok(new { msg = Message });
                    }
                }
                else
                {
                    Message = "Please login!";
                    TempData["errorMessage"] = Message;
                    TempData.Keep("errorMessage");
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return View("Error");
            }
        }
        #endregion

        #region "UpdateProfile"
        [HttpGet, HttpPost]
        public async Task<IActionResult> UpdateProfile(MediaPost objMediaPost, IFormFile ProfilePhoto, string HdnView = "")
        {
            string Message = "";
            try
            {
                string currentActionUrl = HttpContext.Request.Path;
                TempData["currentActionUrl"] = currentActionUrl;
                TempData.Keep("currentActionUrl");

                var UserId = _sessionService.GetInt32("UserId");
                var UserName = _sessionService.GetString("Username");
                if (UserId != null && UserName != null)
                {
                    if (HdnView == "1")
                    {
                        string Url = baseUrl + $"api/AccountAPI/UpdateUserProfile?Id={UserId}";
                        HttpResponseMessage res = await _httpClient.GetAsync(Url);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            MediaPost resData = JsonConvert.DeserializeObject<MediaPost>(resBody);
                            if (resData != null)
                            {
                                ViewBag.ProfilePhotoPath = resData.ProfilePhotoPath;
                            }
                            return View(resData);
                        }
                        else
                        {
                            return BadRequest("Error in updating the Data here.");
                        }
                    }
                    else
                    {
                        if (objMediaPost.hdnId == 1)
                        {
                            if (ProfilePhoto != null)
                            {
                                string Filename = Path.GetFileName(ProfilePhoto.FileName);
                                string FileData = Path.Combine("wwwroot", "images", "UserData");

                                if (!Directory.Exists(FileData))
                                {
                                    Directory.CreateDirectory(FileData);
                                }
                                string FilePath = Path.Combine(FileData, Filename);
                                using (var Stream = new FileStream(FilePath, FileMode.Create))
                                {
                                    await ProfilePhoto.CopyToAsync(Stream);
                                }
                                objMediaPost.ProfilePhotoPath = "/images/UserData/" + Filename;
                            }
                            objMediaPost.Id = UserId;
                            var UpdateData = new
                            {
                                objMediaPost.Id,
                                objMediaPost.ProfilePhotoPath,
                                objMediaPost.Bio,
                                objMediaPost.Mobile,
                                objMediaPost.Fullname,
                                objMediaPost.DateOfBirth
                            };

                            string JsonContent = JsonConvert.SerializeObject(UpdateData);
                            StringContent content = new StringContent(JsonContent, Encoding.UTF8, "application/json");

                            string PostUrl = baseUrl + "api/AccountAPI/UpdateProfilePost";
                            HttpResponseMessage res = await _httpClient.PostAsync(PostUrl, content);
                            if (res.IsSuccessStatusCode)
                            {
                                string resBody = await res.Content.ReadAsStringAsync();
                                dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                                Message = resData.msg;
                                TempData["successMessage"] = Message;
                                TempData.Keep("successMessage");

                                return RedirectToAction("UserAccountPage", "Account");
                            }
                            else
                            {
                                return BadRequest("Unable to update the Data, Please check the issue.");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("UserAccountPage", "Account");

                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return View("Error");
            }

        }
        #endregion

        #region "UserFollow Code"

        [HttpPost]
        public async Task<IActionResult> CheckFollowStatus(string CheckId = "", string Checkusername = "")
        {
            bool isFollowing = false;

            try
            {
                string currentActionUrl = HttpContext.Request.Path;
                TempData["currentActionUrl"] = currentActionUrl;
                TempData.Keep("currentActionUrl");
                var UserId = _sessionService.GetInt32("UserId");
                var Username = _sessionService.GetString("Username");

                if (UserId != null && Username != null)
                {
                    if (!string.IsNullOrEmpty(CheckId))
                    {
                        string url = baseUrl + $"api/AccountAPI/CheckFollowStatus";
                        MediaPost obj = new MediaPost
                        {
                            Username = Username,
                            Id = Convert.ToInt32(CheckId),
                            hdnId = UserId,
                            HdnUsername = Checkusername
                        };

                        var jsonData = JsonConvert.SerializeObject(obj);
                        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                            isFollowing = Convert.ToBoolean(resData.isFollowing);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            return Json(new { isFollowing });
        }



        [HttpPost]
        public async Task<IActionResult> UserFollowup(string Id = "", string hdnUsername = "")
        {
            string Message = "";
            bool response = false;
            bool isFollowing = false;
            try
            {
                string currentActionUrl = HttpContext.Request.Path;
                TempData["currentActionUrl"] = currentActionUrl;
                TempData.Keep("currentActionUrl");
                var UserId = _sessionService.GetInt32("UserId");
                var Username = _sessionService.GetString("Username");
                if (UserId != null && Username != null)
                {
                    if (Id != null)
                    {
                        string url = baseUrl + $"api/AccountAPI/UserFollowup";
                        MediaPost obj = new MediaPost();
                        obj.Username = Username;
                        obj.Id = Convert.ToInt32(Id);
                        obj.hdnId = UserId;
                        obj.HdnUsername = hdnUsername;


                        var jsonData = new
                        {
                            obj.Username,
                            obj.Id,
                            obj.hdnId,
                            obj.HdnUsername
                        };
                        string jsondata = JsonConvert.SerializeObject(jsonData);
                        StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                            response = true;
                            Message = resData.msg;
                            TempData["successMessage"] = Message;
                            //TempData.Keep("successMessage");
                            isFollowing = Convert.ToBoolean(resData.isFollowing);
                        }
                    }
                    else
                    {
                        Message = "Please Select the user to Follow";
                    }
                }
                else
                {

                    TempData["errorMessage"] = Message;
                    TempData.Keep("errorMessage");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Json(new { response, isFollowing });
        }

        #endregion

        #region "Delete Post"
        [HttpDelete]
        public async Task<IActionResult> DeletePost(int id)
        {
            string Message = "";
            bool response = false;
            try
            {
                if (id != null)
                {
                    string deleteUrl = baseUrl + $"api/UtilityAPI/DeletePost?Id={id}";
                    HttpResponseMessage res = await _httpClient.DeleteAsync(deleteUrl);
                    if (res.IsSuccessStatusCode)
                    {

                        string resBody = await res.Content.ReadAsStringAsync();
                        dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                        response = resData.res;
                        if (response == true)
                        {
                            Message = resData.msg;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                response = false;
            }


            return Json(new { msg = Message, res = response });
        }
        #endregion
    }
}
