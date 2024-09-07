//Controllers/ChatController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebRTCService.Data;
using WebRTCService.Models;
using WebRTCService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebRTCService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ApplicationDbContext _context;

        public ChatController(IChatService chatService, ApplicationDbContext context)
        {
            _chatService = chatService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetRoom()
        {
            var rooms = await _chatService.GetRooms();
            return Ok(rooms);
        }        
        
        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoom([FromBody] ChatRoom room)
        {
            var createdRoom = await _chatService.CreateRoom(room);
            return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
        }

        [HttpGet("get-rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _chatService.GetRooms();
            return Ok(rooms);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            Console.WriteLine("in SendMessage"); 
            if (message == null || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Message content cannot be empty.");
            }

            await _chatService.SendMessage(message.RoomId, message.Content);
            return Ok();
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            var rooms = await _context.ChatRooms.ToListAsync();
            return Ok($"Found {rooms.Count} rooms");
        }
    }
}