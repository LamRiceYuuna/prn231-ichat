using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class Profile : BaseEntity {
        [Key]
        public long ProfileId {  get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } //Cần có unique
        public string NickName { get; set; }
        public string AvatarUrl {  get; set; }

    }
}
