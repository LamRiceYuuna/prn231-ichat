namespace WebAPI.DTOs.Notifications {
    public class NotificationResponse {
        public string Type {  get; set; }
        public string Title {  get; set; }
        public string Status { get; set; }
        public NotificationFriendResponse NotificationFriendResponse { get; set; }
        public long NotificationId { get; internal set; }
    }
}
