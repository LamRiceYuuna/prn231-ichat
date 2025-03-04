using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models {
    public class Role : BaseEntity{
        [Key]
        public int RoleId {  get; set; }
        [Required]
        [Range(3, 10)]
        public string RoleName { get; set; }
        public virtual ICollection<User>? Users { get; set;}
        public virtual ICollection<ChatMember>? ChatMembers { get; set;}
    }
}
