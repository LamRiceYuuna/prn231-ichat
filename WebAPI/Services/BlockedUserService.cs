using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class BlockedUserService : BaseService<BlockedUser, BlockedUserRepository> {
        public BlockedUserService(BlockedUserRepository repository) : base(repository) {
        }

        public async Task<BlockedUser> BlockUserAsync(string uuid, string uuidF)
        {
            try
            {
                var blockedUser = await _repository.BlockUserAsync(uuid, uuidF);
                return blockedUser;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UnblockUserAsync(string uuid, string uuidF)
        {
            return await _repository.UnblockUserAsync(uuid, uuidF);
        }
    }
}
