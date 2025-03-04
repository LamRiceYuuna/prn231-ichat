using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using WebAPI.DTOs.Chats;
using WebAPI.DTOs.Friendships;
using WebAPI.Services;

namespace WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FriendShipController : ControllerBase
    {
        private readonly FriendshipService _friendshipService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public FriendShipController(FriendshipService friendshipService, UserService userService, IMapper mapper)
        {
            _friendshipService = friendshipService;
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize("User")]
        [HttpGet("friends_for_chat")]
        public async Task<IActionResult> GetFriendsForChat([FromQuery] string chatType)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            var friends = await _friendshipService.GetFriendsForChatAsync(user, chatType);
            var friendDtos = _mapper.Map<List<FriendDto>>(friends);
            foreach (var item in friendDtos)
            {
                item.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = item.AvatarUrl }, Request.Scheme);
            }
            return Ok(friendDtos);
        }

        [Authorize("User")]
        [HttpGet("friends_not_in_chat/{chatUUID}")]
        public async Task<IActionResult> GetFriendsNotInGroupChat(string chatUUID)
        {
            var user = await _userService.GetCurrentUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }
            var friends = await _friendshipService.GetFriendsNotInGroupChat(user, chatUUID);
            var friendDtos = _mapper.Map<List<FriendDto>>(friends);
            return Ok(friendDtos);
        }
    }
}
