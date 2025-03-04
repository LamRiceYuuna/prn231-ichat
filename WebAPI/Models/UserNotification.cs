using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class UserNotification : BaseEntity{
        //thiết lập cặp key
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public long NotificationId {  get; set; }
        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }
    }
}
