namespace WebAPI.DTOs.Friendships
{
    public class FriendDto
    {
        public long UserId { get; set; }
        public string UUID { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
