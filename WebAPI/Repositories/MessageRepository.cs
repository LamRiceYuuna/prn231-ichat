using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class MessageRepository : BaseRepository<Message, TDbContext> {
        public MessageRepository(TDbContext dbContext) : base(dbContext) {
        }
        public async Task<Message> GetMessageByUUIDAsync(string messageUUID) {
            return _dbContext.Messages.Include(m => m.ChatMember)
                                        .ThenInclude(cm => cm.User)
                                       .FirstOrDefault(m => m.UUID == messageUUID);
        }
    }
}
