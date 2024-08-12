using Microsoft.AspNetCore.Mvc;

namespace Social_Media_Project.Controllers
{
    public class MessangerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChatMessanger()
        {
            string Message = "";
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return BadRequest(Message);
            }
        }
    }
}
