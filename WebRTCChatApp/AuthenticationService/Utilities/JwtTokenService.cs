using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTokenService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(configuration), "JWT Secret Key cannot be null");
        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "JWT Issuer cannot be null");
        _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration), "JWT Audience cannot be null");
        Console.WriteLine($"Configuration loaded. Secret Key: {_secretKey}, Issuer: {_issuer}, Audience: {_audience}");
    }

    public string GenerateToken(User user)
    {
       var tokenHandler = new JwtSecurityTokenHandler();
        
        // Ensure the key is not null
        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new InvalidOperationException("Secret key is not provided.");
        }

        Console.WriteLine($"Generating token for user: {user.Username}");
        Console.WriteLine($"Token expiration: {DateTime.UtcNow.AddHours(8)}");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username), // Use ClaimTypes.Name
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)), 
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        Console.WriteLine($"Generated Token: {tokenHandler.WriteToken(token)}");
        Console.WriteLine($"Token creation date: {DateTime.UtcNow}");
         return tokenHandler.WriteToken(token);
    }
}