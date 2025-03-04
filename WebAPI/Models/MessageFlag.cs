using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class MessageFlag : BaseEntity{
        [Key]
        public long MessageFlagId {  get; set; }
        public long MessageId {  get; set; }
        [ForeignKey("MessageId")]
        public Message Message { get; set; }
        public long ChatMemberId {  get; set; }
        [ForeignKey("ChatMemberId")]
        public ChatMember ChatMember { get; set; }

    }
}
