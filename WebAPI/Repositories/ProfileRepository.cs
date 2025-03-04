using Microsoft.EntityFrameworkCore;
using WebAPI.Constants;
using WebAPI.DTOs.Users;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class ProfileRepository : BaseRepository<Profile, TDbContext> {
        public ProfileRepository(TDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Profile>> GetFriendsInGroupByChatUUID(User user, string chatUUID)
        {
            var chats = await _dbContext.Chats
                            .Where(c => c.Status == Status.ACTIVE && c.UUID == chatUUID)
                            .Include(c => c.ChatMembers)
                            .ThenInclude(cm => cm.User)
                            .ThenInclude(u => u.Profile)
                            .ToListAsync();

            var profiles = chats
                            .SelectMany(c => c.ChatMembers) 
                            .Where(cm => cm.UserId != user.UserId) 
                            .Select(cm => cm.User.Profile) 
                            .Distinct() 
                            .ToList();

            return profiles;
        }
        public async Task<Profile> UpdateProfile(UserProfileDTO profile)
        {

            var user = await _dbContext.Profiles.FindAsync(profile.UserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.NickName = profile.NickName;

            await _dbContext.SaveChangesAsync();

            return user;

        }
    }

}
