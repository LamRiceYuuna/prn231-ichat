using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Constants;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class ChatMemberRepository : BaseRepository<ChatMember, TDbContext> {
        public ChatMemberRepository(TDbContext dbContext) : base(dbContext) {
        }

        public async Task MuteChatMember(int chatMemberId, int? time)
        {
            var chatMember = await _dbContext.ChatMembers
                                              .FirstOrDefaultAsync(cm => cm.ChatMemberId == chatMemberId);

            if (chatMember != null)
            {
                chatMember.Mute = "On";
                await _dbContext.SaveChangesAsync();

                if (time.HasValue && time.Value != 0)
                {
                    await Task.Delay(TimeSpan.FromMinutes(time.Value));

                    var updatedChatMember = await _dbContext.ChatMembers
                                                            .FirstOrDefaultAsync(cm => cm.ChatMemberId == chatMemberId);

                    if (updatedChatMember != null && updatedChatMember.Mute == "On")
                    {
                        updatedChatMember.Mute = "Off";
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<bool> UnmuteChatMember(int chatMemberId)
        {
            var chatMember = await _dbContext.ChatMembers
                                              .FirstOrDefaultAsync(cm => cm.ChatMemberId == chatMemberId);

            if (chatMember != null)
            {
                chatMember.Mute = "Off";
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ChatMember> GetChatMember(int chatMemberId)
        {
            var chatMember = await _dbContext.ChatMembers
                                              .FirstOrDefaultAsync(cm => cm.ChatMemberId == chatMemberId);

            return chatMember;
        }

      
        public async Task<List<string>> GetCommonChatIdsAsync(string uuid1, string uuid2)
        {
            var user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.UUID == uuid1);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.UUID == uuid2);
           

            var chatIdsUser1 = await _dbContext.ChatMembers
                .Where(cm => cm.UserId == user1.UserId)
                .Select(cm => cm.ChatId)
                .ToListAsync();

            var chatIdsUser2 = await _dbContext.ChatMembers
                .Where(cm => cm.UserId == user2.UserId)
                .Select(cm => cm.ChatId)
                .ToListAsync();

           
            var commonChatIds = chatIdsUser1.Intersect(chatIdsUser2).ToList();

            var commonChatUuids = await _dbContext.Chats
            .Where(c => commonChatIds.Contains(c.ChatId) && !c.IsGroup)
            .Select(c => c.UUID)
            .ToListAsync();

            return commonChatUuids;
        }
      
        public async Task<ChatMember> GetChatMemberByIdAsync(long chatMemberId) {
            return await _dbContext.ChatMembers
                .Include(cm => cm.User)
                .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(cm => cm.ChatMemberId == chatMemberId);
        }

        public async Task<long> GetChatMemberIdByChatUUID(User user, string chatUUID)
        {
            var chat = await _dbContext.Chats
                .Where(c => c.Status == Status.ACTIVE && c.UUID == chatUUID)
                .Include(c => c.ChatMembers)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync();

            var chatMember = chat.ChatMembers.FirstOrDefault(cm => cm.UserId == user.UserId);

            return chatMember.ChatMemberId;
        }
    }
}
