using WebAPI.Models;

namespace WebAPI.Repositories {
    public class UserAuthProviderRepository : BaseRepository<UserAuthProvider, TDbContext> {
        public UserAuthProviderRepository(TDbContext dbContext) : base(dbContext) {
        }
    }
}
