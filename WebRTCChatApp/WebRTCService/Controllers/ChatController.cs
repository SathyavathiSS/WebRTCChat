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

        // Constructor injecting IChatService and ApplicationDbContext
        public ChatController(IChatService chatService, ApplicationDbContext context)
        {
            _chatService = chatService;
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of chat rooms.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of all available chat rooms.
        /// </remarks>
        /// <response code="200">Returns a list of chat rooms</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetRoom()
        {
            // Get the list of chat rooms using the chat service
            var rooms = await _chatService.GetRooms();
            return Ok(rooms); // Return 200 OK with the list of rooms
        }        
        
                /// <summary>
        /// Creates a new chat room.
        /// </summary>
        /// <remarks>
        /// This endpoint allows the creation of a new chat room by providing the room's details in the request body.
        /// </remarks>
        /// <param name="room">The chat room to be created</param>
        /// <response code="201">Chat room created successfully</response>
        /// <response code="400">Invalid request, such as missing required data</response>
        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoom([FromBody] ChatRoom room)
        {
            // Create a new chat room using the chat service
            var createdRoom = await _chatService.CreateRoom(room);
            return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
        }

        /// <summary>
        /// Retrieves all chat rooms.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of all chat rooms.
        /// </remarks>
        /// <response code="200">Returns a list of chat rooms</response>
        [HttpGet("get-rooms")]
        public async Task<IActionResult> GetRooms()
        {
            // Get the list of chat rooms using the chat service
            var rooms = await _chatService.GetRooms();
            return Ok(rooms); // Return 200 OK with the list of rooms
        }

        /// <summary>
        /// Sends a message to a specific chat room.
        /// </summary>
        /// <remarks>
        /// This endpoint allows sending a message to a specified chat room. The room ID and message content must be provided in the request body.
        /// </remarks>
        /// <param name="message">The message containing the room ID and content</param>
        /// <response code="200">Message sent successfully</response>
        /// <response code="400">Invalid request, such as missing or empty message content</response>
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            Console.WriteLine("in SendMessage"); 
             // Check if the message content is empty
            if (message == null || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Message content cannot be empty.");// Return 400 Bad Request if message content is empty
            }

            // Send the message to the specified room using the chat service
            await _chatService.SendMessage(message.RoomId, message.Content);
            return Ok(); // Return 200 OK on success
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            var rooms = await _context.ChatRooms.ToListAsync();
            return Ok($"Found {rooms.Count} rooms");
        }
    }
}