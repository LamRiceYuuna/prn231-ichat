using WebAPI.DTOs.Users;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class ProfileService : BaseService<Profile, ProfileRepository> {
        public ProfileService(ProfileRepository repository) : base(repository) {
        }

        public async Task<List<Profile>> GetFriendsInGroupByChatUUID(User user, string chatUUID)
        {
            return await _repository.GetFriendsInGroupByChatUUID(user, chatUUID);
        }
        public async Task<Profile> UpdateProfile(UserProfileDTO profile)
        {
            var user = await _repository.UpdateProfile(profile);
            return user;
        }
    }
}
