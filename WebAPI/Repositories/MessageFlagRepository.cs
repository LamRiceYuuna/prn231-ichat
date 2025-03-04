using WebAPI.Models;

namespace WebAPI.Repositories {
    public class MessageFlagRepository : BaseRepository<MessageFlag, TDbContext> {
        public MessageFlagRepository(TDbContext dbContext) : base(dbContext) {
        }
    }
}
