using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.DTOs.Profiles
{
    public class ProfileDto
    {
        public long ProfileId { get; set; }
        public long UserId { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
