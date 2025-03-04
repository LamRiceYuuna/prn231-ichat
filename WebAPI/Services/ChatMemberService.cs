using WebAPI.Authentication;
using WebAPI.DTOs.Files;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class ChatMemberService : BaseService<ChatMember, ChatMemberRepository> {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtHandler _jwtHandler;
        public ChatMemberService(ChatMemberRepository repository, JwtHandler jwtHandler, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _jwtHandler = jwtHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task MuteChatMember(int chatMemberId, int? time)
        {
            await _repository.MuteChatMember(chatMemberId, time);
        }

        public async Task<bool> UnmuteChatMember(int chatMemberId)
        {
            bool result = await _repository.UnmuteChatMember(chatMemberId);

            return result;
        }

        public async Task<ChatMember> GetChatMember(int chatMemberId)
        {
            var result = await _repository.GetChatMember(chatMemberId);

            return result;
        }

        public async Task<long> GetChatMemberIdByChatUUID(User user, string chatUUID)
        {
            var result = await _repository.GetChatMemberIdByChatUUID(user, chatUUID);

            return result;
        }

        public async Task<List<string>> GetCommonChatIdsAsync(string uuid1, string uuid2)
        {
            return await _repository.GetCommonChatIdsAsync(uuid1, uuid2);
        }
    }
}
