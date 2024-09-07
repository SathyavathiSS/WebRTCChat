//Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using WebRTCService.Services;

namespace WebRTCService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }
    

        public async Task SendMessage(string roomId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                int roomIdInt;
                if (int.TryParse(roomId, out roomIdInt))
                {
                    await _chatService.SendMessage(roomIdInt, content);
                    await Clients.Group(roomId).SendAsync("ReceiveMessage", roomId, content);
                }
                else
                {
                    // Handle invalid room ID
                    await Clients.Caller.SendAsync("Error", "Invalid room ID");
                }
            }
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }
    }

}