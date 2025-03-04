using Microsoft.EntityFrameworkCore;
using WebAPI.Constants;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class BlockedUserRepository : BaseRepository<BlockedUser, TDbContext> {
        public BlockedUserRepository(TDbContext dbContext) : base(dbContext) {

        }

        public async Task<BlockedUser> BlockUserAsync(string uuid, string uuidF)
        {
            var user1 = await _dbContext.Users.FirstOrDefaultAsync(a => a.UUID == uuid);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(a => a.UUID == uuidF);

            var blocked = new BlockedUser
            {
                UserId = user1.UserId,
                BlockedId = user2.UserId,
                Status = Status.ACTIVE,
            };

            // Cập nhật trạng thái của các bản ghi trong bảng Friendship thành "Rejected"
            var friendships = await _dbContext.Friendships
                .Where(f => (f.UserId == user1.UserId && f.FriendUserId == user2.UserId) ||
                            (f.UserId == user2.UserId && f.FriendUserId == user1.UserId))
                .ToListAsync();

            foreach (var friendship in friendships)
            {
                friendship.Status = "Rejected";
            }

            _dbContext.BlockedUsers.Add(blocked);
            await _dbContext.SaveChangesAsync();

            return blocked;
        }

        public async Task<bool> UnblockUserAsync(string uuid, string uuidF)
        {
            var user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.UUID == uuid);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.UUID == uuidF);

            if (user1 == null || user2 == null)
            {
                return false; 
            }

            var blockedUser = await _dbContext.BlockedUsers
                .FirstOrDefaultAsync(bu => bu.UserId == user1.UserId && bu.BlockedId == user2.UserId);

            if (blockedUser == null)
            {
                return false;
            }

            // Cập nhật trạng thái của các bản ghi trong bảng Friendship thành "Rejected"
            var friendships = await _dbContext.Friendships
                .Where(f => (f.UserId == user1.UserId && f.FriendUserId == user2.UserId) ||
                            (f.UserId == user2.UserId && f.FriendUserId == user1.UserId))
                .ToListAsync();

            foreach (var friendship in friendships)
            {
                friendship.Status = "Rejected";
            }

            _dbContext.BlockedUsers.Remove(blockedUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
