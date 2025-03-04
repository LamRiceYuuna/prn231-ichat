using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase {
        private readonly MessageService _messageService;
        public MessageController(MessageService messageService) {
            _messageService = messageService;
        }
    }
}
