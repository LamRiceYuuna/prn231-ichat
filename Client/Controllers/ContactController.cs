using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet("/User/Contacts")]
        public IActionResult Contacts()
        {
            return View("~/Views/User/Contacts.cshtml");
        }
    }
}
