using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class Chat : BaseEntity{
        [Key]
        public long ChatId { get; set; }
        [Required]
        public string UUID { get; set; }
        public string ChatName {  get; set; }
        public bool IsGroup {  get; set; } = false;
        public string AvatarUrl {  get; set; }
        public virtual ICollection<ChatMember> ChatMembers { get; set; }

        // Thêm các thuộc tính để chứa thông tin tin nhắn cuối cùng
        [NotMapped]
        public Message? LastMessage { get; set; }
        [NotMapped]
        public DateTime? LastMessageSentTime { get; set; }
        [NotMapped]
        public bool? LastMessageIsRead { get; set; }
        [NotMapped]
        public User? OtherUser { get; set; }
    }
}
