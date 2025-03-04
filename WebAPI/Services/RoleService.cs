using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class RoleService : BaseService<Role, RoleRepository> {
        public RoleService(RoleRepository repository) : base(repository) {
        }
    }
}
