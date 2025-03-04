using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebAPI.Models;

namespace WebAPI.DTOs.Users
{
    public class UserProfileDTO
    {
        public long UserId { get; set; }
        public string UUID { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public bool HasPassword { get; set; }
        public long ProfileId { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
