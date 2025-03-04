using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using WebAPI.Constants;
using WebAPI.DTOs.Chats;
using WebAPI.Mappers;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        private readonly UserService _userService;
        private readonly FileService _fileService;
        private readonly IMapper _mapper;
        public ChatController(ChatService chatService, UserService userService, FileService fileService, IMapper mapper)
        {
            _chatService = chatService;
            _userService = userService;
            _fileService = fileService;
            _mapper = mapper;
        }
        
        [Authorize("User")]
        [HttpGet("{chatUUID}/{pageNumber}")]
        public async Task<IActionResult> GetChat(string chatUUID, int pageNumber) {
            var user = await _userService.GetCurrentUserAsync();
            var chat = await _chatService.GetChatByUUIDAsync(chatUUID, user, pageNumber);
            var chatRs = _mapper.Map<ChatResponse>(chat);
            if (chat == null) {
                return NoContent();
            }
            
            chatRs.MessageResponse.ForEach(message => { 
                message.FileResponse.ForEach(fileResponse => {
                    if (fileResponse.Type == FileType.IMAGE) {
                        fileResponse.Path = Url.Action("GetImageInChat", "File", new { path = fileResponse.Path }, Request.Scheme);
                    } else if(fileResponse.Type == FileType.VIDEO) {
                        fileResponse.Path = Url.Action("GetVideoInChat", "File", new { path = fileResponse.Path }, Request.Scheme);
                    } else {
                        fileResponse.Path = Url.Action("DownLoadDocumentInchat", "File", new { path = fileResponse.Path }, Request.Scheme);
                    }
                });
                message.UserResponse.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = message.UserResponse.AvatarUrl }, Request.Scheme);
            });
            if (chatRs.IsGroup) {
                chatRs.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = chatRs.AvatarUrl }, Request.Scheme);
            }
            if (chatRs.MessageResponse.Count <=1 && !chatRs.IsGroup) {
                var chatEmpty = await _chatService.GetChatByUUIDAsync(chatUUID);
                var friendUser = chatEmpty.ChatMembers.FirstOrDefault(cm => cm.User.UUID != user.UUID).User;
                chatRs.ChatName = friendUser.Profile.NickName;
                chatRs.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = friendUser.Profile.AvatarUrl }, Request.Scheme);
            }
            return Ok(chatRs);
        }

        [Authorize("User")]
        [HttpGet("chats_user")]
        public async Task<IActionResult> GetUserChatsWithDetails()
        {
            var user = await _userService.GetCurrentUserAsync();
            var chats = await _chatService.GetUserChatsWithDetailsAsync(user);
            var chatResponses = _mapper.Map<List<ChatResponseDTO>>(chats, opt => opt.Items["UserId"] = user.UserId);
            foreach (var item in chatResponses)
            {
                item.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = item.AvatarUrl }, Request.Scheme);
                if(item.OtherUser!= null) {
                    item.OtherUser.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = item.OtherUser.AvatarUrl }, Request.Scheme);
                }
            }
            return Ok(chatResponses);
        }

        [HttpGet("chats_user/{chatUUID}")]
        public async Task<IActionResult> GetUserChatsWithDetailsByChatUUID(String chatUUID)
        {
            var user = await _userService.GetCurrentUserAsync();
            var chats = await _chatService.GetUserChatsWithDetailsAsyncByChatUUID(user, chatUUID);
            var chatResponses = _mapper.Map<ChatResponseDTO>(chats, opt => opt.Items["UserId"] = user.UserId);

            chatResponses.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = chatResponses.AvatarUrl }, Request.Scheme);
                if (chatResponses.OtherUser != null)
                {
                chatResponses.OtherUser.AvatarUrl = Url.Action("GetImageInChat", "File", new { path = chatResponses.OtherUser.AvatarUrl }, Request.Scheme);
                }
            
            return Ok(chatResponses);
        }

        [Authorize("User")]
        [HttpPost("create_chat")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var createdChat = await _chatService.CreateChatWithMembersAsync(request.ChatType, user, request.Friends);
            var chatResponse = _mapper.Map<ChatResponseDTO>(createdChat);

            return Ok(chatResponse);
        }

        [HttpPost("add_friends_to_group/{chatUUID}")]
        public async Task<IActionResult> AddFriendsToGroup(string chatUUID, [FromBody] List<long> friendIds)
        {
            // Kiểm tra tính hợp lệ của tham số
            if (friendIds == null || !friendIds.Any())
            {
                return BadRequest("Friend IDs cannot be null or empty.");
            }

            try
            {
                var createdChat = await _chatService.AddFriendsToGroup(chatUUID, friendIds);
                var chatResponse = _mapper.Map<ChatResponseDTO>(createdChat);

                return Ok(chatResponse);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi phù hợp
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [Authorize]
        [HttpGet("search/{chatUUID}/{content}")]
        public async Task<IActionResult> SearchMessage(string chatUUID, string content)
        {
            var user = await _userService.GetCurrentUserAsync();
            var dictionary = await _chatService.SearchPageWithMessageContentAsync(chatUUID, content, user);
            return Ok(dictionary);
        }

        [Authorize("User")]
        [HttpDelete("delete_chat/{chatUUID}")]
        public async Task<IActionResult> DeleteChat(string chatUUID)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _chatService.SetChatInactiveAsync(chatUUID, user);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize("User")]
        [HttpPost("CreateChatMessage/{uuidU}/{uuidF}")]
        public async Task<IActionResult> CreateChatIndividual(string uuidU, string uuidF)
        {
            if (uuidU == null || uuidF == null)
            {
                return Unauthorized();
            }

            var createdChat = await _chatService.CreateChatIndividual(uuidU, uuidF);
            var chatResponse = _mapper.Map<ChatResponseDTO>(createdChat);

            return Ok(chatResponse);
        }
    }
}
