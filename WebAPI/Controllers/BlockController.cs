using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Constants;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {   private readonly BlockedUserService lockedUserService;
        public BlockController(BlockedUserService lockedUserService) 
        {
            this.lockedUserService = lockedUserService;
        }

        [Authorize]
        [HttpPost("Blocked/{uuidF}")]
        public async Task<IActionResult> AddFriend(string uuidF)
        {
            var userUUID = User?.FindFirst(JwtType.UUID)?.Value;
            if (string.IsNullOrEmpty(uuidF))
            {
                return BadRequest("Invalid data.");
            }
            try
            {
                var friendship = await lockedUserService.BlockUserAsync(userUUID, uuidF);
                if (friendship != null)
                {
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


        [Authorize]
        [HttpDelete("UnBlock/{uuidF}")]
        public async Task<IActionResult> UnBlock(string uuidF)
        {
            var userUUID = User?.FindFirst(JwtType.UUID)?.Value;
            if (string.IsNullOrEmpty(uuidF))
            {
                return BadRequest("Invalid data.");
            }
            try
            {
                var friendship = await lockedUserService.UnblockUserAsync(userUUID, uuidF);
                if (friendship != null)
                {
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
    }
}
