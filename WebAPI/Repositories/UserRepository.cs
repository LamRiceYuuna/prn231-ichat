using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Constants;
using WebAPI.DTOs.Users;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class UserRepository : BaseRepository<User, TDbContext>
    {
        public UserRepository(TDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _dbContext.Users.Include(u => u.Profile)
                                            .Include(r => r.Role)
                                            .FirstOrDefaultAsync(u => u.UserName == username);
            return user;
        }

        public async Task<User> GetUserByUUIDAsync(string UUID)
        {
            var user = await _dbContext.Users.Include(u => u.Profile)
                                            .Include(u => u.Role)
                                            .Include(u => u.ChatMembers)
                                            .ThenInclude(cm => cm.Chat)
                                            .FirstOrDefaultAsync(u => u.UUID == UUID);
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email) {
            var user = await _dbContext.Users
                .Include(u => u.Profile)
                .Include(r => r.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<User> GetUserIncludeFriendShip(string UUID)
        {
            var user = await _dbContext.Users.Include(u => u.Profile)
                                            .Include(u => u.Friendships)
                                            .ThenInclude(u => u.FriendUser)
                                            .ThenInclude(u => u.Profile)
                                            .Include(u => u.BlockedUsers)
                                            .ThenInclude(u => u.Blocked)
                                            .ThenInclude(u => u.Profile)
                                           .FirstOrDefaultAsync(u => u.UUID == UUID);
            if (user != null)
            {
                user.Friendships = user.Friendships.Where(f => f.Status == "Accepted").ToList();
            }

            return user;
        }

        public async Task<List<User>> GetFriendRequestsForCurrentUser(string currentUserUuid)
        {
            var userList = await _dbContext.Friendships
                .Where(f => f.FriendUser.UUID == currentUserUuid && f.Status == "Pending")
                .Include(f => f.User) // Include the User navigation property
                .ThenInclude(u => u.Profile) // Include the Profile navigation property of User
                .Select(f => f.User) // Select the user who sent the friend request
                .ToListAsync();

            return userList;
        }

        public async Task<List<User>> GetUsersWithoutFriendship(string currentUserUUID)
        {
            var currentUser = await _dbContext.Users
                                              .Include(u => u.Friendships)
                                              .Include(u => u.FriendshipUsers)
                                              .FirstOrDefaultAsync(u => u.UUID == currentUserUUID);

            if (currentUser == null)
            {
                return new List<User>();
            }

            // Lấy ra danh sách các UserId mà currentUser đã kết bạn
            var friendIds = _dbContext.Friendships
                                      .Where(f => (f.UserId == currentUser.UserId || f.FriendUserId == currentUser.UserId) && f.Status == "Accepted")
                                      .Select(f => f.UserId == currentUser.UserId ? f.FriendUserId : f.UserId)
                                      .ToList();

            

            var userList = await _dbContext.Users
                                   .Include(u => u.Profile)
                                   .Include(u => u.Friendships)
                                   .Where(u => u.UUID != currentUserUUID)
                                   .Where(u => !friendIds.Contains(u.UserId) ||
                                               _dbContext.Friendships.Any(f =>
                                                   (f.UserId == currentUser.UserId && f.FriendUserId == u.UserId && (f.Status == "Rejected" || f.Status == "Pending")) ||
                                                   (f.FriendUserId == currentUser.UserId && f.UserId == u.UserId && (f.Status == "Rejected" || f.Status == "Pending"))
                                               ))
                                   .ToListAsync();

            return userList;
        }


        public async Task<User> GetUserInforByUUID(string UUID)
        {
            var user = await _dbContext.Users.Include(u => u.Profile)
                                            .FirstOrDefaultAsync(u => u.UUID == UUID);
            return user;
        }

        public async Task<IEnumerable<User>> GetMutualFriends(string uuid1, string uuid2)
        {
            var user1Id = _dbContext.Users.Where(a => a.UUID == uuid1).Select(a => a.UserId).FirstOrDefault();
            var user2Id = _dbContext.Users.Where(a => a.UUID == uuid2).Select(a => a.UserId).FirstOrDefault();


            var friendsOfUser1 = _dbContext.Friendships
                .Where(b => (b.UserId == user1Id || b.FriendUserId == user1Id) && b.Status == "Accepted")
                .Select(b => b.UserId == user1Id ? b.FriendUserId : b.UserId);


            var friendsOfUser2 = _dbContext.Friendships
                .Where(b => (b.UserId == user2Id || b.FriendUserId == user2Id) && b.Status == "Accepted")
                .Select(b => b.UserId == user2Id ? b.FriendUserId : b.UserId);


            var mutualFriendIds = friendsOfUser1.Intersect(friendsOfUser2);

            var mutualFriends = _dbContext.Users.Include(x => x.Profile)
                .Where(a => mutualFriendIds.Contains(a.UserId))
                .ToList();

            return mutualFriends;
        }

        public async Task<List<User>> GetFriendsByUuid(string uuid)
        {
            var user = await _dbContext.Users
                                       .FirstOrDefaultAsync(u => u.UUID == uuid);

            if (user == null)
            {
                return new List<User>();
            }

            // Lấy ra các Friendships mà User là người gửi hoặc người nhận và có trạng thái "Accepted"
            var friends = await _dbContext.Friendships
                                          .Where(f => (f.UserId == user.UserId || f.FriendUserId == user.UserId) && f.Status == "Accepted")
                                          .Include(f => f.User)
                                          .ThenInclude(u => u.Profile)
                                          .Include(f => f.FriendUser)
                                          .ThenInclude(fu => fu.Profile)
                                          .Select(f => f.UserId == user.UserId ? f.FriendUser : f.User)
                                          .ToListAsync();

            return friends;
        }
        public async Task<User> GetProfile(long id)
        {
            var user = await _dbContext.Users.Include(u => u.Profile)
                                            .FirstOrDefaultAsync(u => u.UserId == id);
            return user;
        }

        public async Task<User> UpdateProfile(UserProfileDTO profile)
        {
          
                var user = await _dbContext.Users.FindAsync(profile.UserId);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.UserName = profile.UserName;
                user.Password = profile.Password;

                await _dbContext.SaveChangesAsync();

                return user;
            
        }


        public async Task<User> GetUserByUUIDIncludeNotificationAsync(string? userUUID) {
            var user = await _dbContext.Users.Include(u => u.UserNotifications)
                                                .ThenInclude(un => un.Notification)
                                                .FirstOrDefaultAsync(u => u.UUID == userUUID);
            return user;
        }

        public async Task<IEnumerable<User>> GetUserIncludeStatisticAsync() {
            DateTime startOfWeek = GetStartOfWeek(DateTime.UtcNow, DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(7);
            var users = await _dbContext.Users
                .Include(u => u.Profile)
                .Include(u => u.Statistics.Where(s => s.LoginAt >= startOfWeek && s.LoginAt < endOfWeek))
                .ToListAsync();
            return users;
        }
        private DateTime GetStartOfWeek(DateTime dt, DayOfWeek startOfWeek) {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public async Task<IEnumerable<User>> GetAllUserAsync() {
            return await _dbContext.Users.Include(u => u.Profile)
                                         .Include(u => u.Role)
                                         .Where(u => u.Role.RoleName != RoleType.ADMIN)
                                         .ToListAsync();
        }
    }
}
