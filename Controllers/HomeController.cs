using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.AppCode;
using Social_Media_Project.Models;
using System.Diagnostics;

namespace Social_Media_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISessionService _sessionService;

        public HomeController(ILogger<HomeController> logger, ISessionService sessionService)
        {
            _logger = logger;
            _sessionService = sessionService;
        }



        public IActionResult Index()
        {
            string Message = "";
            try
            {
                var Username = _sessionService.GetString("Username");
                var UserId = _sessionService.GetInt32("UserId");
                if (Username != null && UserId != null)
                {
                    var currentActionUrl = TempData["currentActionUrl"].ToString();
                    TempData.Remove("currentActionUrl"); 
                    return Redirect(currentActionUrl);
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                Message = ex.Message;
                TempData["errorMessage"] = Message;
                TempData.Keep("errorMessage");
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ClearSession()
        {
            _sessionService.Remove("WelcomeMessage");
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
