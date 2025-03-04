using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class UserAuthProviderService : BaseService<UserAuthProvider, UserAuthProviderRepository> {
        public UserAuthProviderService(UserAuthProviderRepository repository) : base(repository) {
        }
    }
}
