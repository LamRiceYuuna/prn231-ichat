using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class Friendship : BaseEntity{
        public long UserId {  get; set; }
        [ForeignKey("UserId")]  
        public User User { get; set; }
        public long FriendUserId {  get; set; }
        [ForeignKey("FriendUserId")]
        public User FriendUser { get; set; }
    }
}
