using Microsoft.AspNetCore.SignalR;
using WebRTCService.Models;
using WebRTCService.Services;

namespace WebRTCService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }
    
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("WebSocket connection established: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogError(exception, "WebSocket connection closed: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string roomId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                int roomIdInt;
                if (int.TryParse(roomId, out roomIdInt))
                {
                    try
                    {
                        await _chatService.SendMessage(roomIdInt, content);
                        await Clients.Group(roomId).SendAsync("ReceiveMessage", content);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending message");
                    }
                }
            }
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            _logger.LogInformation("User {ConnectionId} joined room {RoomId}", Context.ConnectionId, roomId);
        }

        public async Task SendOffer(string peerId, RTCSessionDescription offer)
        {
            await Clients.Client(peerId).SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string peerId, RTCSessionDescription answer)
        {
            await Clients.Client(peerId).SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendICECandidate(string peerId, RTCIceCandidate candidate)
        {
            await Clients.Client(peerId).SendAsync("ReceiveICECandidate", candidate);
        }
    }
}
