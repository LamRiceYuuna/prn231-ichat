using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ProfileController : Controller
    {
        [HttpGet("/User/Profile")]
        public IActionResult Index()
        {
            return View("~/Views/User/Profile.cshtml");
        }
    }
}
