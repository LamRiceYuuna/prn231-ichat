using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Chats;
using WebAPI.DTOs.Profiles;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public ProfileController(ProfileService profileService, UserService userService, IMapper mapper)
        {
            _profileService = profileService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("friends_in_group/{chatUUID}")]
        public async Task<IActionResult> GetFriendsInGroupByChatUUID(String chatUUID)
        {
            var user = await _userService.GetCurrentUserAsync();
            var friends = await _profileService.GetFriendsInGroupByChatUUID(user, chatUUID);
            var friendResponses = _mapper.Map<List<ProfileDto>>(friends);
            return Ok(friendResponses);
        }
    }
}
