using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Social_Media_Project.AppCode;
using Social_Media_Project.Models;
using System.Text;

namespace Social_Media_Project.Controllers
{
    public class UtilityController : Controller
    {


        private readonly HttpClient _httpClient;
        IHttpContextAccessor _httpContextAccessor;
        private readonly dynamic baseUrl;

        private readonly ISessionService _sessionService;
        public UtilityController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
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




        #region "Like Post Section"
        [HttpPost]
        public async Task<IActionResult> LikePost(string Id = "", string postUsername = "")
        {
            string Message = "";
            bool response = false;
            bool IsLiked = false;
            MediaPost obj = new MediaPost();
            try
            {

                var Username = _sessionService.GetString("Username");
                var UserId = _sessionService.GetInt32("UserId");

                if (UserId != null && Username != null)
                {
                    if (Id != null)
                    {
                        obj.PostId = Convert.ToInt32(Id);
                        obj.Id = UserId;
                        obj.Username = Username;
                        obj.HdnUsername = postUsername;
                        var jsonData = new
                        {
                            obj.PostId,
                            obj.Id,
                            obj.Username,
                            obj.HdnUsername
                        };

                        string jsonContent = JsonConvert.SerializeObject(jsonData);
                        StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        string url = baseUrl + "api/UtilityAPI/LikePost";
                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                            response = resData.res;
                            Message = resData.msg;
                            IsLiked = resData.isLike;

                        }
                    }
                    else
                    {
                        Message = "Please select the Post to take the Action";
                        response = false;
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please login!";
                    TempData.Keep("errorMessage");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                response = false;
            }
            return Json(new { response, Message, IsLiked });
        }


        [HttpPost]
        public async Task<IActionResult> LikePostCheck(string id = "", string postUsername = "")
        {
            string Message = "";
            bool response = false;
            try
            {
                var Username = _sessionService.GetString("Username");
                var UserId = _sessionService.GetInt32("UserId");

                if (UserId != null && Username != null)
                {
                    string url = baseUrl + "api/UtilityAPI/CheckLikeStatus";
                    MediaPost obj = new MediaPost();
                    obj.PostId = Convert.ToInt32(id);
                    obj.Id = UserId;
                    obj.Username = Username;
                    obj.HdnUsername = postUsername;
                    var jsonData = new
                    {
                        obj.PostId,
                        obj.Id,
                        obj.Username,
                        obj.HdnUsername
                    };

                    string jsonContent = JsonConvert.SerializeObject(jsonData);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);
                        Message = resData.msg;
                        response = resData.isLiked;
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please login!";
                    TempData.Keep("errorMessage");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                response = false;
                Message = ex.Message;
            }
            return Json(new { response, Message });
        }
        #endregion

        #region "Add Comment Section"
        [HttpPost]
        public async Task<IActionResult> AddCommentSection(int postId = 0, string comment = "", int userIdd = 0, string postUsername = "")
        {
            string Message = "";
            bool response = false;
            try
            {
                var Username = _sessionService.GetString("Username");
                var UserId = _sessionService.GetInt32("UserId");

                if (UserId != null && Username != null)
                {
                    if (postId != null && !string.IsNullOrWhiteSpace(comment) && userIdd != null && !string.IsNullOrWhiteSpace(postUsername))
                    {
                        string url = baseUrl + "api/UtilityAPI/AddCommentSectionAPI";

                        Comment objComment = new Comment();

                        objComment.PostId = postId;
                        objComment.CommentInput = comment;
                        objComment.PostCommentId = UserId;
                        objComment.PostCommentUsername = Username;
                        objComment.PostAddId = userIdd;
                        objComment.PostAddUsername = postUsername;
                        var objCommentData = new
                        {
                            objComment.PostId,
                            objComment.CommentInput,
                            objComment.PostCommentId,
                            objComment.PostCommentUsername,
                            objComment.PostAddId,
                            objComment.PostAddUsername

                        };
                        string jsonData = JsonConvert.SerializeObject(objComment);
                        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        //API Call
                        HttpResponseMessage res = await _httpClient.PostAsync(url, content);
                        if (res.IsSuccessStatusCode)
                        {
                            string resBody = await res.Content.ReadAsStringAsync();
                            dynamic resData = JsonConvert.DeserializeObject<dynamic>(resBody);

                            Message = resData.msg;
                            response = resData.res;
                        }

                    }
                    else
                    {
                        Message = "Please pass the required Parameter!";
                        response = false;
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please login!";
                    TempData.Keep("errorMessage");
                    return RedirectToAction("Index", "Home");
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

        #region "Get Comment Section"

        [HttpGet]
        public async Task<IActionResult> GetCommentView(int Id = 0)
        {
            string Message = "";
            bool response = false;
            try
            {
                if (Id != null)
                {
                    string Url = baseUrl + $"api/UtilityAPI/GetCommentViewAPI?Id={Id}";

                    HttpResponseMessage res = await _httpClient.GetAsync(Url);
                    if (res.IsSuccessStatusCode)
                    {
                        string resBody = await res.Content.ReadAsStringAsync();
                        List<Comment> lstComment = JsonConvert.DeserializeObject<List<Comment>>(resBody);
                        if (lstComment.Any(m => m.CommentInput != null))
                        {
                            Message = "Comment retrieved successfully.";
                            response = true;
                            List<Comment> lstData = lstComment;
                            return Json(new { msg = Message, response = response, comment = lstData });
                        }
                        else
                        {
                            Message = "No comments found.";
                            response = false;
                            return Json(new { msg = Message, response = response });
                        }


                    }

                }
                else
                {
                    Message = "Please pass the required Parameter!";
                    response = true;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                response = false;
            }
            return Json(new { msg = Message, ress = response });
        }

        #endregion
    }
}
