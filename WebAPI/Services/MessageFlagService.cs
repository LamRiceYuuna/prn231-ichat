using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class MessageFlagService : BaseService<MessageFlag, MessageFlagRepository> {
        public MessageFlagService(MessageFlagRepository repository) : base(repository) {
        }
    }
}
