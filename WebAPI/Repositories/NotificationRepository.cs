using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class NotificationRepository : BaseRepository<Notification, TDbContext> {
        public NotificationRepository(TDbContext dbContext) : base(dbContext) {
        }

        public async Task<bool> IsFriend(string uuid, string uuidF) {
            var fs = await _dbContext.Friendships.Include(f => f.User)
                .Include(f => f.FriendUser)
                .FirstOrDefaultAsync(f => f.User.UUID == uuid && f.FriendUser.UUID == uuidF);
            return fs != null;
        }
    }
}
