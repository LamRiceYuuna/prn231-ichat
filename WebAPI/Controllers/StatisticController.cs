using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase{
        private readonly UserService _userService;
        public StatisticController(UserService userService) {
            _userService = userService;
        }
        [Authorize("Admin")]
        [HttpGet("load")]
        public async Task<IActionResult> GetStatisticUserInWeekAsync() {
            var s = await _userService.GetStatisticUseInWeekAsync();
            return Ok(s);
        }
    }
}
