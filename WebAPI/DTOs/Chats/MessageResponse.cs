namespace WebAPI.DTOs.Chats {
    public class MessageResponse {
        public string MessageUUID { get; set; }
        public UserResponse UserResponse { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool IsEdit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<FileResponse> FileResponse {  get; set; } = new List<FileResponse>();
    }
}