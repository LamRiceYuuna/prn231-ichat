using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models {
    public class Notification : BaseEntity {
        [Key]
        public long NotificationId { get; set; }
        public string Type {  get; set; }
        public string Title { get; set; }
        public string Message {  get; set; }
    }
}
