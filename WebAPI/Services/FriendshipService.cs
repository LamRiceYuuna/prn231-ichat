using WebAPI.Constants;
using AutoMapper;
using WebAPI.DTOs.Friendships;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class FriendshipService : BaseService<Friendship, FriendshipRepository> {
        private readonly IMapper _mapper;
        public FriendshipService(FriendshipRepository repository, IMapper mapper) : base(repository) {
            _mapper = mapper;
        }
        public async Task<List<FriendDto>> GetFriendsForChatAsync(User user, string chatType)
        {
            var friends = await _repository.GetFriendsForChatAsync(user, chatType);
            return _mapper.Map<List<FriendDto>>(friends);
        }

        public async Task<Friendship> AddFriendAsync(string uuid, string uuidF)
        {
            try
            {
                var friendShip = await _repository.AddFriendAsync(uuid, uuidF);
                return friendShip;
            }
            catch (Exception)
            {
                return null;
            }          
        }

        public async Task<bool> UpdateFriendshipStatusAsync(string userUUID, string friendUUID)
        {
            var friendship = await _repository.GetFriendshipAsync(userUUID, friendUUID);
            if (friendship == null)
            {
                return false;
            }

            if (friendship.Status == Status.PENDING)
            {
                friendship.Status = Status.REJECTED;
            }
            else if (friendship.Status == Status.REJECTED)
            {
                friendship.Status = Status.PENDING;
            }
            else
            {
                return false; 
            }

            await _repository.UpdateFriendshipStatusAsync(friendship);
            return true;
        }


        public async Task<bool> UpdateFriendshipRequestAsync(string userUUID, string friendUUID, int requestCode)
        {
            var friendship = await _repository.GetFriendshipAsync(userUUID, friendUUID);
            if (friendship == null)
            {
                return false;
            }


            if (requestCode == 1)
            {
                friendship.Status = Status.ACCEPTED;
            }
            else if (requestCode == 2)
            {
                friendship.Status = Status.REJECTED;
            }
            else
            {
                return false;
            }

            await _repository.UpdateFriendshipStatusAsync(friendship);
            return true;
        }     

        public async Task<List<FriendDto>> GetFriendsNotInGroupChat(User user, string chatUUID)
        {
            var friends = await _repository.GetFriendsNotInGroupChat(user, chatUUID);
            return _mapper.Map<List<FriendDto>>(friends);
        }
    }
}
