using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.ChatMembers;
using WebAPI.DTOs.Chats;
using WebAPI.DTOs.Files;
using WebAPI.Models;
using WebAPI.Repositories;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMemberController : ControllerBase
    {
        private readonly ChatMemberService _chatMemberService;
        private readonly UserService _userService;
        private readonly IMapper mapper;
        
        public ChatMemberController(ChatMemberService chatMemberService, UserService userService, IMapper mapper)
        {
            _chatMemberService = chatMemberService;
            _userService = userService;
            this.mapper = mapper;
        }

        [HttpPost("MuteChatMember/{chatMemberId}/{time?}")]
        public async Task<IActionResult> MuteChatMember(int chatMemberId, int? time)
        {
            try
            {
                int muteTime = time ?? 0;
                await _chatMemberService.MuteChatMember(chatMemberId, muteTime);

                return Ok("Mute completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UnmuteChatMember/{chatMemberId}")]
        public async Task<IActionResult> UnmuteChatMember(int chatMemberId)
        {
            try
            {
                bool result = await _chatMemberService.UnmuteChatMember(chatMemberId);

                if (result)
                {
                    return Ok("Unmute success");
                }
                else
                {
                    return NotFound("Chat member not found or already unmuted");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetChatMember/{chatMemberId}")]
        public async Task<IActionResult> GetFileByChatMemberId(int chatMemberId)
        {
            var chatMember = await _chatMemberService.GetChatMember(chatMemberId);
            var chatMemberDto = mapper.Map<ChatMemberResponse>(chatMember);

            return Ok(chatMemberDto);
        }

        [Authorize("User")]
        [HttpGet("GetChatMemberId/{chatUUID}")]
        public async Task<IActionResult> GetChatMemberIdByChatUUID(string chatUUID)
        {
            var user = await _userService.GetCurrentUserAsync();
            var chatMemberId = await _chatMemberService.GetChatMemberIdByChatUUID(user, chatUUID);

            return Ok(chatMemberId);
        }

        [HttpGet("GetChatUUID/{uuid1}/{uuid2}")]
        public async Task<IActionResult> GetCommonChatIds(string uuid1, string uuid2)
        {
            try
            {
                var chatuuiIds = await _chatMemberService.GetCommonChatIdsAsync(uuid1, uuid2);
                return Ok(chatuuiIds);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
