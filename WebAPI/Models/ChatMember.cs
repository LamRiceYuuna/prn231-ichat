using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class ChatMember {
        public long ChatMemberId {  get; set; }
        public long ChatId {  get; set; }
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }
        public long UserId {  get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string Mute {  get; set; } //Tắt tiếng
        [ForeignKey("RoleId")]
        public Role Role { get; set; } //Vai trò trong group
        public int RoleId { get; set; }
        public DateTime JoinedAt {  get; set; } = DateTime.UtcNow; // Tham gia lúc nào
        public DateTime? LeftAt { get; set;} // rời khỏi lúc nào
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageFlag> MessageFlags { get; set; }
    }
}
