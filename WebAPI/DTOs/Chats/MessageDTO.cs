namespace WebAPI.DTOs.Chats
{
    public class MessageDTO
    {
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsRead { get; set; }
    }
}
