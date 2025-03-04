using WebAPI.Models;

namespace WebAPI.Repositories {
    public class RoleRepository : BaseRepository<Role, TDbContext> {
        public RoleRepository(TDbContext dbContext) : base(dbContext) {
        }
        public async Task<Role> GetRoleByRoleName(string roleName) {
            var role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == roleName);
            return role;
        }
    }
}
