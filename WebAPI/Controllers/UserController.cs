using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebAPI.Constants;
using WebAPI.DTOs.Users;
using WebAPI.Hubs;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Authentication;
using AutoMapper;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase {
        private readonly UserService _userService;
        private readonly IHubContext<UserHub> _userHubContext;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly ProfileService _profileservice;
        public UserController(UserService userService, IHubContext<UserHub> userHubContext, JwtHandler jwtHandler, IMapper mapper, ProfileService profileservice) {
            _userService = userService;
            _userHubContext = userHubContext;
            _jwtHandler = jwtHandler;
            _mapper = mapper;
            _profileservice = profileservice;
        }

        //Hàm chỉ để test, không sử dụng
        [Authorize]
        [HttpGet("userinfo")]
        public IActionResult GetUserInfo() {
            var username = User?.FindFirst(JwtType.USERNAME)?.Value;
            return Ok(new { Username = username });
        }
        //Done
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest) {
            var rs = await _userService.Login(loginRequest);
            return Ok(rs);
        }

        [HttpGet("signin-google")]
        public IActionResult SignInGoogle(string returnUrl) {
            var properties = new AuthenticationProperties {
                RedirectUri = Url.Action("GoogleResponse", "User", new { returnUrl = returnUrl }, Request.Scheme)
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl) {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) {
                return Unauthorized();
            }

            var claims = result.Principal?.Claims;
            if (claims == null) {
                return BadRequest("No claims found");
            }

            var cookies = HttpContext.Request.Cookies.Keys.ToList();
            foreach (var cookie in cookies) {
                Response.Cookies.Delete(cookie);
            }
            var user = await _userService.CreateUserFromGoooleClaimsAsync(claims);
            if (user.Status == Status.BANNED) {
                return Redirect(returnUrl);
            }
            await _userService.UpdateLatLoginAsync(user);

            user.Profile.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = user.Profile.AvatarUrl }, Request.Scheme);
            string token = _jwtHandler.GenerateToken(user);
            Response.Cookies.Append("bearer_token", token, new CookieOptions {
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Redirect(returnUrl);
        }
        [Authorize]
        [HttpGet("getuser")]
        public async Task<IActionResult> GetUserByUUID() {
            User user = await _userService.GetUserIncludeFriendShip();

            if (user == null) {
                return BadRequest("User not found!");
            }

            var userDto = _mapper.Map<UserDTO>(user);
            userDto.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = userDto.AvatarUrl }, Request.Scheme);
            foreach (var item in userDto.Friendships) {
                item.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = item.AvatarUrl }, Request.Scheme);
            }
            foreach (var item in userDto.BlockedUsers) {
                item.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = item.AvatarUrl }, Request.Scheme);
            }
            return Ok(userDto);
        }

        [Authorize]
        [HttpGet("getAllUser")]
        public async Task<ActionResult<List<UserFriendDTO>>> GetAllUser() {
            var userUUID = User?.FindFirst(JwtType.UUID)?.Value;
            var users = await _userService.GetAllUser();
            var usersd = users.Select(u => new UserFriendDTO {
                UUID = u.UUID,
                UserName = u.UserName,
                NickName = u.Profile.NickName,
                AvatarUrl = Url.Action("GetImageInChat", "File", new { path = u.Profile.AvatarUrl }, Request.Scheme),
                FriendStatus =
                 (u.FriendshipUsers ?? new List<Friendship>())
                    .Where(f => (f.User.UUID == userUUID && f.FriendUserId == u.UserId) ||
                                (f.FriendUser.UUID == userUUID && f.UserId == u.UserId))
                    .Select(f => f.Status)
                    .FirstOrDefault() ?? "No"
            }).ToList();

            return Ok(usersd);
        }

        [HttpGet("{UUID}")]
        public async Task<ActionResult<UserDTO>> GetUserByUUID(string UUID) {
            var users = await _userService.GetUserInforByUUID(UUID);
            if (users == null) {
                return BadRequest();
            }

            var userDtos = _mapper.Map<UserInfoDTO>(users);
            userDtos.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = userDtos.AvatarUrl }, Request.Scheme);
            return Ok(userDtos);
        }


        [HttpGet("GetMutualFriends/{UUID1}/{UUID2}")]
        public async Task<ActionResult<UserDTO>> GetMutualFriends(string UUID1, string UUID2) {
            var usersMutual = await _userService.GetMutualFriends(UUID1, UUID2);
            if (usersMutual == null) {
                return BadRequest();
            }

            var userDtos = _mapper.Map<List<UserInfoDTO>>(usersMutual);
            foreach (var f in userDtos) {
                f.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = f.AvatarUrl }, Request.Scheme);
            }
            return Ok(userDtos);
        }

        [Authorize]
        [HttpGet("FriendRequest")]
        public async Task<IActionResult> GetFriendRequestsForCurrentUser() {
            var userUUID = User?.FindFirst(JwtType.UUID)?.Value;
            if (string.IsNullOrEmpty(userUUID)) {
                return BadRequest("UUID is required.");
            }

            var friendRequests = await _userService.GetFriendRequestsForCurrentUser(userUUID);

            if (friendRequests == null || !friendRequests.Any()) {
                return NotFound("No friend requests found.");
            }
            var userDtos = _mapper.Map<List<UserDTO>>(friendRequests);
            foreach (var f in userDtos) {
                f.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = f.AvatarUrl }, Request.Scheme);
            }
            return Ok(userDtos);
        }

        [Authorize]
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsByUuid() {
            var userUUID = User?.FindFirst(JwtType.UUID)?.Value;
            var friends = await _userService.GetFriendsByUuid(userUUID);

            if (friends == null || !friends.Any()) {
                return NotFound("No friends found.");
            }

            var fr = _mapper.Map<List<UserInfoDTO>>(friends);
            foreach (var f in fr) {
                f.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = f.AvatarUrl }, Request.Scheme);
            }
            return Ok(fr);
        }
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile() {
            var use = await _userService.GetCurrentUserAsync();
            var userId = use.UserId;
            var profile = await _userService.GetProfile(userId);
            var userProfileDto = new UserProfileDTO {
                UserId = profile.UserId,
                UUID = profile.UUID,
                UserName = profile.UserName,
                Email = profile.Email,
                Password = profile.Password,
                HasPassword = profile.HasPassword,
                ProfileId = profile.Profile.ProfileId,
                NickName = profile.Profile.NickName,
                AvatarUrl = profile.Profile.AvatarUrl
            };
            return Ok(userProfileDto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UserProfileDTO user) {
            await _userService.UpdateProfile(user);
            await _profileservice.UpdateProfile(user);
            return Ok(user);
        }

        [Authorize("Admin")]
        [HttpGet("Load")]
        public async Task<IActionResult> GetUserForAdminAsync() {
            var user = await _userService.GetAllUserAsync();
            return Ok(user);
        }
        [Authorize("Admin")]
        [HttpPut("band-user")]
        public async Task<IActionResult> BandUserAsync(string uuid) {
            var status = await _userService.BandUserAsync(uuid);
            return Ok(status);
        }
    }
}
