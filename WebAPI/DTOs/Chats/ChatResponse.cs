namespace WebAPI.DTOs.Chats {
    public class ChatResponse {
        public string UUID { get; set; }
        public string ChatName {  get; set; }
        public bool IsGroup {  get; set; }
        public string AvatarUrl {  get; set; }
        public List<MessageResponse> MessageResponse { get; set; }
    }
}
