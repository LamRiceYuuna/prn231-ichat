using System.ComponentModel.DataAnnotations;
using WebAPI.Models;

namespace WebAPI.DTOs.Users
{
    public class UserDTO
    {
        public string UUID { get; set; }
        public string? UserName { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public virtual ICollection<FriendshipDTO>? Friendships { get; set; }
        public virtual ICollection<BlockedUserDTO> BlockedUsers { get; set; }
    }
}
