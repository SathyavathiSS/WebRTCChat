using WebRTCService.Models;

namespace WebRTCService.Services
{
    public interface IChatService
    {
        Task<ChatRoom> CreateRoom(ChatRoom room);
        Task<IEnumerable<ChatRoom>> GetRooms();
        Task SendMessage(int roomId, string content);
    }
}
