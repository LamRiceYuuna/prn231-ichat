using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class FriendShipController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
