using WebAPI.Constants;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs.Friendships;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class FriendshipRepository : BaseRepository<Friendship, TDbContext> {
        private readonly IMapper _mapper;
        public FriendshipRepository(TDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<Friendship> AddFriendAsync(string uuid, string uuidF)
        {
            var user1 = await _dbContext.Users.FirstOrDefaultAsync(a => a.UUID == uuid);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(a => a.UUID == uuidF);

            var friendship = new Friendship
            {
                UserId = user1.UserId,
                FriendUserId = user2.UserId,
                Status = Status.PENDING
            };

            _dbContext.Friendships.Add(friendship);
            await _dbContext.SaveChangesAsync();

            return friendship; 
        }

        public async Task<Friendship> GetFriendshipAsync(string userUUID, string friendUUID)
        {
            var friendship = await _dbContext.Friendships
                .Include(f => f.User)
                .Include(f => f.FriendUser)
                .FirstOrDefaultAsync(f => (f.User.UUID == userUUID && f.FriendUser.UUID == friendUUID) ||
                                          (f.FriendUser.UUID == userUUID && f.User.UUID == friendUUID));
            return friendship;
        }

        public async Task UpdateFriendshipStatusAsync(Friendship friendship)
        {
            _dbContext.Friendships.Update(friendship);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<FriendDto>> GetFriendsForChatAsync(User user, string chatType)
        {
            if (chatType == "individual")
            {
                // Lấy danh sách bạn bè có status = Accepted
                var friendsQuery = _dbContext.Friendships
                    .Where(f => f.Status == "Accepted" && (f.UserId == user.UserId || f.FriendUserId == user.UserId))
                    .Include(f => f.User.Profile) // Bao gồm thông tin Profile của User
                    .Include(f => f.FriendUser.Profile) // Bao gồm thông tin Profile của FriendUser
                    .Select(f => f.UserId == user.UserId ? f.FriendUser : f.User);

                // Lấy danh sách bạn bè không tồn tại trong bảng Chat với IsGroup = 0
                var friends = await friendsQuery
                    .ToListAsync();

                var individualFriends = friends
                    .Where(friend => !_dbContext.Chats
                        .Where(c => c.IsGroup == false)
                        .SelectMany(c => c.ChatMembers)
                        .Any(cm => cm.UserId == user.UserId && cm.Chat.ChatMembers.Any(cm2 => cm2.UserId == friend.UserId)))
                    .ToList();

                return _mapper.Map<List<FriendDto>>(individualFriends);
            }
            else if (chatType == "group")
            {
                // Lấy danh sách bạn bè có status = Accepted
                var friends = await _dbContext.Friendships
                    .Where(f => f.Status == "Accepted" && (f.UserId == user.UserId || f.FriendUserId == user.UserId))
                    .Include(f => f.User.Profile) // Bao gồm thông tin Profile của User
                    .Include(f => f.FriendUser.Profile) // Bao gồm thông tin Profile của FriendUser
                    .Select(f => f.UserId == user.UserId ? f.FriendUser : f.User)
                    .ToListAsync();

                return _mapper.Map<List<FriendDto>>(friends);
            }

            return new List<FriendDto>();
        }

        public async Task<List<FriendDto>> GetFriendsNotInGroupChat(User user, string chatUUID)
        {
            var hasRoleAdminInChat = await _dbContext.ChatMembers
                                    .Include(cm => cm.Chat)  
                                    .Include(cm => cm.User)  
                                    .Where(cm => cm.Chat.UUID == chatUUID
                                                 && cm.RoleId == 2
                                                 && cm.UserId == user.UserId)  
                                    .AnyAsync();

            if (!hasRoleAdminInChat)
            {
                return new List<FriendDto>();
            }

            var acceptedFriends = await _dbContext.Friendships
                            .Where(f => f.Status == "Accepted" && (f.UserId == user.UserId || f.FriendUserId == user.UserId))
                            .Include(f => f.User)
                                .ThenInclude(u => u.Profile)
                            .Include(f => f.FriendUser)
                                .ThenInclude(fu => fu.Profile)
                            .ToListAsync();

            var friendsNotInChat = acceptedFriends
                            .Where(f => !_dbContext.Chats
                                .Where(c => c.UUID == chatUUID)
                                .SelectMany(c => c.ChatMembers)
                                .Any(cm => cm.UserId == (f.UserId == user.UserId ? f.FriendUserId : f.UserId)))
                            .Select(f => f.UserId == user.UserId ? f.FriendUser : f.User)
                            .ToList();

            return _mapper.Map<List<FriendDto>>(friendsNotInChat);
        }

    }
}
