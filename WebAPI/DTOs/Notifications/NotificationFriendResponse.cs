namespace WebAPI.DTOs.Notifications
{
    public class NotificationFriendResponse
    {
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime DateAgo { get; set; }
        public string UserSendRequest { get; set; } // UUID của user
    }
}
