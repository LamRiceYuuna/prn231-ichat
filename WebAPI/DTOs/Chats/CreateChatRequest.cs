namespace WebAPI.DTOs.Chats
{
    public class CreateChatRequest
    {
        public string ChatType { get; set; }
        public List<long> Friends { get; set; }
    }
}
