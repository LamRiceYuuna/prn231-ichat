using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.DTOs.ChatMembers
{
    public class ChatMemberResponse
    {
        public long ChatMemberId { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string Mute { get; set; } //Tắt tiếng
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow; // Tham gia lúc nào
        public DateTime LeftAt { get; set; } // rời khỏi lúc nào
    }
}
