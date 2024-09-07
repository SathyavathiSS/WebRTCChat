namespace WebRTCService.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Message>? Messages { get; set; }
    }
}
