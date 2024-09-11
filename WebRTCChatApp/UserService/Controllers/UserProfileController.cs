// Controllers/UserProfileController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using UserService.Models;
using SharedData.Data;
using SharedData.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Constructor injecting ApplicationDbContext and configuration settings
        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Retrieves the authenticated user's profile information.
        /// </summary>
        /// <remarks>
        /// This endpoint requires a valid JWT token to authenticate the user. The token should be sent in the Authorization header.
        /// </remarks>
        /// <response code="200">Returns the user's profile details</response>
        /// <response code="400">Invalid request, such as missing or invalid username</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [Authorize]
        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                // Extract username from JWT claims
                var username = User.FindFirst(ClaimTypes.Name)?.Value; // ClaimTypes.Name represents the username claim
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Invalid username.");
                }
                // Query user from the database
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Map user entity to UserProfileDto for returning profile data
                var profile = new UserProfileDto
                {
                    Username = user.Username,   
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    ProfilePicture = user.ProfilePicture ?? string.Empty
                };

                return Ok(profile);// Return 200 OK with user profile
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");// Return 500 in case of server errors
            }
        }

                /// <summary>
        /// Updates the authenticated user's profile information.
        /// </summary>
        /// <remarks>
        /// This endpoint allows the user to update their profile (email, first name, last name, and profile picture). Requires a valid JWT token.
        /// </remarks>
        /// <param name="model">The profile update information</param>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid request, such as missing or invalid username</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileDto model)
        {
            try
            {
                // Extract username from JWT claims
                var username = User.FindFirst(ClaimTypes.Name)?.Value;  // ClaimTypes.Name represents the username claim
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Invalid username.");
                }

                // Query user from the database
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Update user entity with new profile data
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ProfilePicture = model.ProfilePicture;

                // Save changes to the database
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok("Profile updated successfully.");// Return 200 OK on success
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");// Return 500 in case of server errors
            }
        }
    }
}