namespace WebAPI.DTOs.Users
{
    public class UserFriendDTO
    {
        public string UUID { get; set; }
        public string? UserName { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }

        public string FriendStatus { get; set; }
    }
}
