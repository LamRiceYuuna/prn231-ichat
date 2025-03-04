namespace WebAPI.DTOs.Chats
{
    public class ChatResponseDTO
    {
        public string UUID { get; set; }
        public string ChatName { get; set; }
        public bool IsGroup { get; set; }
        public string AvatarUrl { get; set; }
        public MessageDTO LastMessage { get; set; }
        public DateTime LastMessageSentTime { get; set; }
        public bool LastMessageIsRead { get; set; }
        public UserResponseDTO OtherUser { get; set; }
    }
}
