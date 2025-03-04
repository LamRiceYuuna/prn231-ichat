using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class StoriesController : Controller
    {
        [HttpGet("/User/Stories")]
        public IActionResult Index()
        {
            return View("~/Views/User/Stories.cshtml");
        }
    }
}
