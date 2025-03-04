using Microsoft.EntityFrameworkCore;
using WebAPI.Constants;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class UserNotificationRepository : BaseRepository<UserNotification, TDbContext> {
        public UserNotificationRepository(TDbContext dbContext) : base(dbContext) {
        }
        public async Task<bool> ReadNotificationAllAsync(Dictionary<string, string> dictionary) {
            return await ExecuteInTransactionAsync(async () => {
                foreach (var kvp in dictionary) {
                    var notificationId = kvp.Key;
                    var uuid = kvp.Value;
                    var notifications = await _dbContext.UserNotifications
                        .Where(un => un.User.UUID == uuid && un.NotificationId == int.Parse(notificationId))
                        .ToListAsync();
                    foreach (var notification in notifications) {
                        notification.Status = Status.READ;
                        await _dbContext.SaveChangesAsync();
                    }
                    
                }

                return true;
            });
        }

    }
}
