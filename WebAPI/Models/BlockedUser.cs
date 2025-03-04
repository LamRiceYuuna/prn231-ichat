using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class BlockedUser : BaseEntity {
        public long UserId {  get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public long BlockedId {  get; set; }
        [ForeignKey("BlockedId")]
        public User Blocked {  get; set; }

    }
}
