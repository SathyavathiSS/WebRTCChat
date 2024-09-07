//Controllers/ChatController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebRTCService.Models;
using WebRTCService.Services;

namespace WebRTCService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
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
            if (message == null || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Message content cannot be empty.");
            }

            await _chatService.SendMessage(message.RoomId, message.Content);
            return Ok();
        }
    }
}