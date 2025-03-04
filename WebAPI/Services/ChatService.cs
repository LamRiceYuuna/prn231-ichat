using WebAPI.DTOs.Chats;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class ChatService : BaseService<Chat, ChatRepository> {
        public ChatService(ChatRepository repository) : base(repository) {
        }

        public async Task<Chat> GetChatByUUIDAsync(string chatUUID, User user, int pageNumber = 1) {
            var chat = await _repository.GetChatsByUserWithPaginatedMessagesAsync(chatUUID, user, pageNumber);
            return chat;
        }
        public async Task<List<Chat>> GetUserChatsWithDetailsAsync(User user)
        {
            return await _repository.GetUserChatsWithDetailsAsync(user);
        }

        public async Task<Chat> CreateChatWithMembersAsync(string chatType, User user, List<long> friendIds) {
            return await _repository.CreateChatWithMembersAsync(chatType, user, friendIds);
        }
      
        public async Task<Chat> CreateChatIndividual(string uuidU, string uuidF)
        {
            return await _repository.CreateChatIndividual(uuidU, uuidF);
        }

        public async Task<Dictionary<int, List<string>>> SearchPageWithMessageContentAsync(string chatUUID, string content, User user) {
            var dictionary = await _repository.GetPagesWithMessageContentAsync(chatUUID, content, user);
            return dictionary;
        }
        public async Task<bool> SetChatInactiveAsync(string chatUUID, User user)
        {
            return await _repository.SetChatInactiveAsync(chatUUID, user.UserId);
        }

        public async Task<Chat> GetChatByUUIDAsync(string chatUUID) {
            var chat = await _repository.GetChatByUUIDAsync(chatUUID);
            return chat;
        }

        public async Task<Chat> GetUserChatsWithDetailsAsyncByChatUUID(User user, string chatUUID)
        {
            return await _repository.GetUserChatsWithDetailsAsyncByChatUUID(user, chatUUID);
        }

        public async Task<Chat> AddFriendsToGroup(string chatUUID, List<long> friendIds)
        {
            return await _repository.AddFriendsToGroup(chatUUID, friendIds);
        }

        
    }
}
