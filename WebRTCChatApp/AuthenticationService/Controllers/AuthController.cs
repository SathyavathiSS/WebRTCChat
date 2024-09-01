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
            // Retrieve the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Verify the password
            bool isPasswordValid = _passwordHasher.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _jwtTokenService.GenerateToken(user);

            //return Ok("User logged in successfully.");
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult GetSecureData()
        {
            try
            {
                Console.WriteLine("Secure data endpoint hit.");
                return Ok("This is a secured endpoint");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}