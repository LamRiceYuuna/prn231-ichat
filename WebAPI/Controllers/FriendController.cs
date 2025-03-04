using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAPI.Constants;
using WebAPI.Hubs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Code 1: accept, != 1 reject
    public class FriendController : ControllerBase
    {
        private readonly FriendshipService _friendshipService;
        private readonly IHubContext<UserHub> _userHubContext;
        private readonly NotificationService _notificationService;
        public FriendController (FriendshipService friendshipService, IHubContext<UserHub> userHubContext, NotificationService notificationService)
        {
            _friendshipService = friendshipService;
            _userHubContext = userHubContext;
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpPost("AddFriend/{uuidF}")]
        public async Task<IActionResult> AddFriend(string uuidF)
        {
            var userUUID = User?.FindFirst(JwtType.UUID)?.Value;
            if (string.IsNullOrEmpty(uuidF))
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var friendship = await _friendshipService.AddFriendAsync(userUUID, uuidF);
                if (friendship != null)
                {
                    await _notificationService.AddNotificationAsync(userUUID, uuidF);

                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to add friend.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateStatusFriendShip/{uuidU}/{uuidF}")]
        public async Task<IActionResult> UpdateStatusFriendShip(string uuidU,string uuidF)
        {          
            if (string.IsNullOrEmpty(uuidU) || string.IsNullOrEmpty(uuidF))
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var result = await _friendshipService.UpdateFriendshipStatusAsync(uuidU, uuidF);
                if (result)
                {
                    await _notificationService.AddNotificationAsync(uuidU, uuidF);
                    return Ok("Friendship status updated successfully.");
                }
                else
                {
                    return BadRequest("Failed to update friendship status.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateStatusFriendShip/{uuidU}/{uuidF}/{code}")]
        public async Task<IActionResult> UpdateStatusFriendShipRequest(string uuidU, string uuidF, int code)
        {
            if (string.IsNullOrEmpty(uuidU) || string.IsNullOrEmpty(uuidF))
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var result = await _friendshipService.UpdateFriendshipRequestAsync(uuidU, uuidF, code);
                if (result)
                {
                    await _notificationService.AddNotificationAsync(uuidU, uuidF, code);

                    return Ok("Friendship status updated successfully.");
                }
                else
                {
                    return BadRequest("Failed to update friendship status.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
  
}
