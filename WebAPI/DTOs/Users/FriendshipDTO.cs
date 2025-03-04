namespace WebAPI.DTOs.Users
{
    public class FriendshipDTO
    {
        public string UUID { get; set; }
        public string Username { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
