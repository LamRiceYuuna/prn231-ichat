using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class User : BaseEntity {
        [Key]
        public long UserId { get; set; }
        [Required]
        public string UUID {  get; set; }
        [Range(5, 25)]
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        public bool HasPassword { get; set; }
        public DateTime LastLogin { get; set; }

        public int RoleId {  get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        public Profile Profile { get; set; }
        public virtual ICollection<UserAuthProvider> AuthProviders { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Friendship>? Friendships { get; set; }

        [InverseProperty("FriendUser")]
        public virtual ICollection<Friendship>? FriendshipUsers { get; set; } 
        public virtual ICollection<ChatMember>? ChatMembers { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        public virtual ICollection<Statistic> Statistics { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<BlockedUser> BlockedUsers { get; set; }
        [InverseProperty("Blocked")]
        public virtual ICollection<BlockedUser> UsersBlocked { get; set; }
        public virtual ICollection<Story> Stories { get; set; }
        [InverseProperty("Reporter")]
        public virtual ICollection<Report> Reporters { get; set; }
        [InverseProperty("ReportedUser")]
        public virtual ICollection<Report> ReportedUsers { get; set; }

    }
}
