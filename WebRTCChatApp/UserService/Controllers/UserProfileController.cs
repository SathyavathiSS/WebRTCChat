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

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [Authorize]
        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value; // Use ClaimTypes.Name for username
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Invalid username.");
                }
                //var user = await _context.Users.FindAsync(username);
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Map user entity to UserProfileDto
                var profile = new UserProfileDto
                {
                    Username = user.Username,   
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    ProfilePicture = user.ProfilePicture ?? string.Empty
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        //endpoint to update user profile
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileDto model)
        {
            try
            {
                // Use int.TryParse to safely parse the user ID
                var username = User.FindFirst(ClaimTypes.Name)?.Value;  // Use ClaimTypes.Name for username
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Invalid username.");
                }

                //var user = await _context.Users.FindAsync(username);
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

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}