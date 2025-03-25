using Microsoft.AspNetCore.Mvc;

namespace LabProject.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (username == "admin" && password == "1234") 
            {
                return RedirectToAction("Dashboard", "Home"); // Başarılı girişte yönlendir
            }

            ViewBag.ErrorMessage = "Invalid username or password";
            return View();
        }
    }
}
