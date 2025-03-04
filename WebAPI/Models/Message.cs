using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class Message : BaseEntity {
        [Key]
        public long MessageId {  get; set; }
        public string UUID {  get; set; }
        public long ChatMemberId {  get; set; }
        [ForeignKey("ChatMemberId")]
        public ChatMember ChatMember {  get; set; }
        public string Content {  get; set; }
        public string ContentType {  get; set; }
        public bool IsEdited {  get; set; } = false;
        public virtual ICollection<File>? Files { get; set; }
        public virtual ICollection<MessageFlag> MessageFlags { get; set; } = new List<MessageFlag>();

    }
}
