using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class UserNotificationService : BaseService<UserNotification, UserNotificationRepository> {
        public UserNotificationService(UserNotificationRepository repository) : base(repository) {
        }

        public async Task<bool> ReadNotificationAllAsync(Dictionary<string, string> notifications) {
            return await _repository.ReadNotificationAllAsync(notifications);
        }
    }
}
