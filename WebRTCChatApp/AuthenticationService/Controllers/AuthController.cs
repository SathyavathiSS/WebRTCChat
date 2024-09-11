// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AuthenticationService.Utilities;
using Microsoft.AspNetCore.Authorization;
using SharedData.Data;
using SharedData.Models;

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

        // Constructor to inject dependencies
        public AuthController(JwtTokenService jwtTokenService, ApplicationDbContext context, IConfiguration configuration)
        {
            _jwtTokenService = jwtTokenService;
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">User registration model containing username and password.</param>
        /// <returns>Status 200 if successful, 400 if username already exists.</returns>
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

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="model">Login model containing username and password.</param>
        /// <returns>A JWT token if successful, 401 if authentication fails.</returns>
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

        /// <summary>
        /// A test endpoint to verify the validity of the JWT token.
        /// </summary>
        /// <returns>A secured message if the token is valid.</returns>
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
    }
}