using System.Net;

namespace WebAPI.DTOs.Chats {
    public class MessageRequest {
        public string ChatUUID { get; set; }
        public string MessageType { get; set; }
        public string? Content {  get; set; }
        public List<FileRequest>? FileRequests {  get; set; } 
    }
}
