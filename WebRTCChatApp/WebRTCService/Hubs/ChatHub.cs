//Hubs/ChatHub.cs
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
            _logger.LogInformation("WebSocket connection established");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogError(exception, "WebSocket connection closed");
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
                        await Clients.Group(roomId).SendAsync("ReceiveMessage", roomId, content);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception
                        Console.WriteLine($"Error sending message: {ex.Message}");
                        await Clients.Caller.SendAsync("Error", $"Failed to send message: {ex.Message}");
                    }
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
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error joining room: {ex.Message}");
                await Clients.Caller.SendAsync("Error", $"Failed to join room: {ex.Message}");
            }
        }

        public async Task CreateRoom(string roomName)
        {
            try
            {
                // Create a new ChatRoom object
                var newRoom = new ChatRoom
                {
                    Name = roomName
                };

                var createdRoom = await _chatService.CreateRoom(newRoom);
                if (createdRoom != null)
                {
                    await BroadcastNewRoom(createdRoom);
                    await Clients.Others.SendAsync("NewRoomCreated", createdRoom);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "Failed to create room.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating room: {ex.Message}");
                await Clients.Caller.SendAsync("Error", $"Failed to create room: {ex.Message}");
            }
        }

        private async Task BroadcastNewRoom(ChatRoom newRoom)
        {
            await Clients.All.SendAsync("RefreshRoomList");
        }
    }
}