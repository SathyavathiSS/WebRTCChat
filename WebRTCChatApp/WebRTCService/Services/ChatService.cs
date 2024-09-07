using Microsoft.EntityFrameworkCore;
using WebRTCService.Models;
using WebRTCService.Data;

namespace WebRTCService.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatRoom> CreateRoom(ChatRoom room)
        {
            await _context.ChatRooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<IEnumerable<ChatRoom>> GetRooms()
        {
            return await _context.ChatRooms.ToListAsync();
        }

        public async Task SendMessage(int roomId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                await _context.Messages.AddAsync(new Message
                {
                    RoomId = roomId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Content cannot be empty.");
            }
        }
    }
}
