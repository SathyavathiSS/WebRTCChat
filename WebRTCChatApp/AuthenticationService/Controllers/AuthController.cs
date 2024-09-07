// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using AuthenticationService.Utilities;
using Microsoft.AspNetCore.Authorization;
using AuthenticationService.Models;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(JwtTokenService jwtTokenService, ApplicationDbContext context, IConfiguration configuration)
        {
            _jwtTokenService = jwtTokenService;
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest("Username already exists.");
            }

            // Generate a salt and hash the password
            byte[] salt = _passwordHasher.GenerateSalt();
            byte[] passwordHash = _passwordHasher.HashPasswordWithSalt(model.Password, salt);

            var user = new User
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                PasswordSalt = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Invalid login model.");
            }

            // Retrieve the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                Console.WriteLine($"No user found with the given username: {model.Username}");
                return Unauthorized("Invalid username or password.");
            }
            // Log to check retrieved user info
            Console.WriteLine($"User found: {user.Username}, checking password...");

            // Verify the password
            bool isPasswordValid = _passwordHasher.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                Console.WriteLine($"Password verification failed for user: {model.Username}");
                return Unauthorized("Invalid username or password.");
            }

            var token = _jwtTokenService.GenerateToken(user);
            Console.WriteLine($"Generated Token: {token}");

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("test-token")]
        public IActionResult GetSecureData()
        {
            try
            {
                Console.WriteLine("Secure data endpoint hit.");
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Invalid username.");
                }
                Console.WriteLine($"Authenticated user: {username}");
                return Ok("This is a secured endpoint");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
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
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePicture = user.ProfilePicture
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