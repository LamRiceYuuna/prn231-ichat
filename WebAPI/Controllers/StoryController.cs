using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Constants;
using WebAPI.DTOs.Story;
using WebAPI.DTOs.Users;
using WebAPI.Hubs;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly StoryService _storyService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public StoryController(StoryService storyService, UserService userService, IMapper mapper)
        {
            _storyService = storyService;
            _userService = userService;
            _mapper = mapper;
        }
        [HttpPost("CreateStory")]
        public async Task<IActionResult> CreateStory(IFormFile file)
        {
            try
            {
                var use = await _userService.GetCurrentUserAsync();
                var userId = use.UserId;
                var story = await _storyService.CreateStoryAsync(file, userId);
                return Ok(new { message = "File uploaded successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpGet("{UUID}")]
        public async Task<ActionResult<UserDTO>> GetStoryByUUID(string UUID)
        {
            var story = await _storyService.GetStoryByUUID(UUID);
            if (story == null)
            {
                return BadRequest();
            }

            var storyDtos = _mapper.Map<List<StoryDTO>>(story);
            foreach (var item in storyDtos)
            {
                item.Path = Url.Action("GetImageInChat", "File", new { item.Path }, Request.Scheme);
            }
            return Ok(storyDtos);
        }

        [HttpDelete("DeleteStory")]
        public async Task<IActionResult> DeleteStory(int id)
        {
            try
            {
                var story = await _storyService.DeleteStory(id);
                return Ok(new { message = "File Delete successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
