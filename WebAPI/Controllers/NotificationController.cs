using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase {
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        public NotificationController(NotificationService notificationService, UserService userService) {
            _notificationService = notificationService;
            _userService = userService;
        }

        [Authorize("User")]
        [HttpGet("load")]
        public async Task<IActionResult> LoadNotification() {
            var user = await _userService.GetCurrentUserIncludeMessageAsync();
            var notifications = await _notificationService.LoadNotificationAsync(user);
            return Ok(notifications);
        }
    }
}
