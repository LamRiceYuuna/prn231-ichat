using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Utilities;

namespace WebAPI.Repositories
{
    public class ChatRepository : BaseRepository<Chat, TDbContext>
    {
        public ChatRepository(TDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<Chat> GetChatsByUserWithPaginatedMessagesAsync(string chatUUID, User user, int pageNumber = 1, int pageSize = 30)
        {
            int skipAmount = (pageNumber - 1) * pageSize;

            var chat = await _dbContext.Chats
                .Where(c => c.UUID == chatUUID && c.ChatMembers.Any(cm => cm.UserId == user.UserId))
                .Include(c => c.ChatMembers)
                    .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                return null;
            }

            var messages = await _dbContext.Messages
                .Where(m => m.ChatMember.ChatId == chat.ChatId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Include(m => m.Files)
                .Include(m => m.ChatMember)
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.Profile)
                .ToListAsync();

            foreach (var chatMember in chat.ChatMembers) {
                chatMember.Messages = messages.Where(m => m.ChatMemberId == chatMember.ChatMemberId && m.Status == Status.ACTIVE).ToList();
            }

            return chat;
        }

        public async Task<List<Chat>> GetUserChatsWithDetailsAsync(User user)
        {
            var chats = await _dbContext.Chats
                .Where(c => c.Status == Status.ACTIVE && c.ChatMembers.Any(cm => cm.UserId == user.UserId))
                .Include(c => c.ChatMembers)
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.Profile)
                .Include(c => c.ChatMembers)
                    .ThenInclude(cm => cm.Messages)
                        .ThenInclude(m => m.MessageFlags)
                .ToListAsync();

            foreach (var chat in chats)
            {
                // Lấy tin nhắn cuối cùng
                var lastMessage = chat.ChatMembers
                    .SelectMany(cm => cm.Messages)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                if (lastMessage != null)
                {
                    chat.LastMessage = lastMessage;
                    chat.LastMessageSentTime = lastMessage.CreatedAt;
                    chat.LastMessageIsRead = lastMessage.MessageFlags.Any(f => f.ChatMember.UserId == user.UserId && f.Status == "Read");
                }

                if (!chat.IsGroup)
                {
                    // Lấy thông tin của người nhắn tin kia
                    var otherUser = chat.ChatMembers
                        .Where(cm => cm.UserId != user.UserId)
                        .Select(cm => cm.User)
                        .FirstOrDefault();
                    if (otherUser != null)
                    {
                        chat.ChatName = otherUser.Profile.NickName;
                        chat.AvatarUrl = otherUser.Profile.AvatarUrl;
                        chat.OtherUser = otherUser;
                    }
                }
            }

            return chats;
        }

        public async Task<Chat> CreateChatWithMembersAsync(string chatType, User user, List<long> friendIds)
        {
            return await ExecuteInTransactionAsync(async () =>
            {
                var chat = new Chat
                {
                    UUID = Guid.NewGuid().ToString(),
                    ChatName = chatType == "group" ? $"Group chat {RandomNumberGenerator.GenerateRandomSixDigitNumber()}" : "",
                    IsGroup = chatType == "group" ? true : false,
                    AvatarUrl = chatType == "group" ? "group_avatar.jpg" : "",
                    Status = Status.ACTIVE,
                };
                await _dbContext.Chats.AddAsync(chat);
                await _dbContext.SaveChangesAsync();

                var chatMembers = new List<ChatMember>
                {
                    new ChatMember
                    {
                        UserId = user.UserId,
                        Mute = MuteType.OFF,
                        RoleId = chatType == "group" ? 2 : 3, // 2: Admin, 3: Member
                        ChatId = chat.ChatId
                    }
                };

                foreach (var friendId in friendIds)
                {
                    chatMembers.Add(new ChatMember
                    {
                        UserId = friendId,
                        Mute = MuteType.OFF,
                        RoleId = 3, // Member
                        ChatId = chat.ChatId
                    });
                }
                await _dbContext.ChatMembers.AddRangeAsync(chatMembers);
                await _dbContext.SaveChangesAsync();

                return chat;
            });
        }



        public async Task<Chat> CreateChatIndividual(string uuidU, string uuidF)
        {
            return await ExecuteInTransactionAsync(async () =>
            {
                var chat = new Chat
                {
                    UUID = Guid.NewGuid().ToString(),
                    ChatName = "",
                    IsGroup = false,
                    AvatarUrl = "",
                    Status = Status.ACTIVE,
                };
                await _dbContext.Chats.AddAsync(chat);
                await _dbContext.SaveChangesAsync();

                var user1 = await _dbContext.Users.FirstOrDefaultAsync(a => a.UUID == uuidU);
                var user2 = await _dbContext.Users.FirstOrDefaultAsync(a => a.UUID == uuidF);

                var chatMembers = new List<ChatMember>
                {
                    new ChatMember
                    {
                        UserId = user1.UserId,
                        Mute = MuteType.OFF,
                        RoleId = 3,
                        ChatId = chat.ChatId
                    }, 
                    new ChatMember
                    {
                        UserId = user2.UserId,
                        Mute = MuteType.OFF,
                        RoleId = 3,
                        ChatId = chat.ChatId
                    }
                   
                };
                
                await _dbContext.ChatMembers.AddRangeAsync(chatMembers);
                await _dbContext.SaveChangesAsync();

                return chat;
            });
        }

        public async Task<Chat> AddFriendsToGroup(string chatUUID, List<long> friendIds)
        {
            return await ExecuteInTransactionAsync(async () =>
            {
                // Tìm cuộc trò chuyện dựa trên UUID
                var chat = await _dbContext.Chats
                    .Where(c => c.UUID == chatUUID)
                    .FirstOrDefaultAsync();

                // Nếu không tìm thấy cuộc trò chuyện với UUID, ném ra ngoại lệ
                if (chat == null)
                {
                    throw new Exception("Chat not found with the provided UUID.");
                }

                // Tạo danh sách thành viên cho cuộc trò chuyện
                var chatMembers = new List<ChatMember>();

                // Thêm tất cả bạn bè vào danh sách thành viên với vai trò Member
                foreach (var friendId in friendIds)
                {
                    chatMembers.Add(new ChatMember
                    {
                        ChatId = chat.ChatId,
                        UserId = friendId,
                        Mute = MuteType.OFF,
                        RoleId = 3, // Member
                    });
                }

                // Thêm các thành viên vào cơ sở dữ liệu
                await _dbContext.ChatMembers.AddRangeAsync(chatMembers);
                await _dbContext.SaveChangesAsync();

                return chat;
            });
        }




        public async Task<Dictionary<int, List<string>>> GetPagesWithMessageContentAsync(string chatUUID, string content, User user, int pageSize = 30) {
            var chat = await _dbContext.Chats
                .Where(c => c.UUID == chatUUID && c.ChatMembers.Any(cm => cm.UserId == user.UserId))
                .Include(c => c.ChatMembers)
                .FirstOrDefaultAsync();

            if (chat == null) {
                return null;
            }

            var allMessages = await _dbContext.Messages
                .Where(m => m.ChatMember.ChatId == chat.ChatId && m.Status == Status.ACTIVE)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            var messagesWithContent = allMessages
                .Where(m => m.Content.Contains(content))
                .ToList();

            var pageMessagesDict = new Dictionary<int, List<string>>();

            foreach (var message in messagesWithContent) {
                var messageIndex = allMessages.FindIndex(m => m.UUID == message.UUID);
                int pageNumber = (messageIndex / pageSize) + 1;

                if (!pageMessagesDict.ContainsKey(pageNumber)) {
                    pageMessagesDict[pageNumber] = new List<string>();
                }

                pageMessagesDict[pageNumber].Add(message.UUID);
            }

            return pageMessagesDict;
        }

        public async Task<Chat> GetUserChatsWithDetailsAsyncByChatUUID(User user, string chatUUID)
        {
            var chat = await _dbContext.Chats
                .Where(c => c.Status == Status.ACTIVE && c.ChatMembers.Any(cm => cm.UserId == user.UserId) && c.UUID == chatUUID)
                .Include(c => c.ChatMembers)
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.Profile)
                .Include(c => c.ChatMembers)
                    .ThenInclude(cm => cm.Messages)
                        .ThenInclude(m => m.MessageFlags)
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                return null; // Trả về null nếu không tìm thấy cuộc trò chuyện
            }

            // Lấy tin nhắn cuối cùng
            var lastMessage = chat.ChatMembers
                .SelectMany(cm => cm.Messages)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            if (lastMessage != null)
            {
                chat.LastMessage = lastMessage;
                chat.LastMessageSentTime = lastMessage.CreatedAt;
                chat.LastMessageIsRead = lastMessage.MessageFlags.Any(f => f.ChatMember.UserId == user.UserId && f.Status == "Read");
            }

            if (!chat.IsGroup)
            {
                // Lấy thông tin của người nhắn tin kia
                var otherUser = chat.ChatMembers
                    .Where(cm => cm.UserId != user.UserId)
                    .Select(cm => cm.User)
                    .FirstOrDefault();
                if (otherUser != null)
                {
                    chat.ChatName = otherUser.Profile.NickName;
                    chat.AvatarUrl = otherUser.Profile.AvatarUrl;
                    chat.OtherUser = otherUser;
                }
            }

            return chat;
        }

        public async Task<bool> SetChatInactiveAsync(string chatUUID, long userId)
        {
            var chat = await _dbContext.Chats
                .Where(c => c.UUID == chatUUID && c.Status == Status.ACTIVE && c.ChatMembers.Any(cm => cm.UserId == userId))
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                return false;
            }

            return await DeleteChatAsync(chat.ChatId);
        }

        public async Task<Chat> GetChatByUUIDAsync(string UUID)
        {
            var chat = await _dbContext.Chats.Include(c => c.ChatMembers)
                                        .ThenInclude(cm => cm.User)
                                        .ThenInclude(u => u.Profile)
                                        .FirstOrDefaultAsync(c => c.UUID == UUID);
            return chat;
        }
    }
}
