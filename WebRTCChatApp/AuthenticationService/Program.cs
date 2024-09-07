using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using System.Security.Claims;

System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidator.GetServerCertificateValidationCallback();

var builder = WebApplication.CreateBuilder(args);
// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Retrieve and validate JWT settings
var jwtKey = builder.Configuration["Jwt:Key"];
 Console.WriteLine($"jwtKey in Program.cs: {jwtKey}");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Validate JWT settings
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration values are missing.");
}

// Add JwtTokenService
builder.Services.AddSingleton<JwtTokenService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new JwtTokenService(configuration);
});

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = ClaimTypes.Name,
        ClockSkew = TimeSpan.Zero // Set this to zero to eliminate potential clock drift issues
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("Authorization failed: " + context.ErrorDescription);
            return Task.CompletedTask;
        }
    };      
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.Use(async (context, next) =>
{
    // Log request
    Console.WriteLine("Request:");
    Console.WriteLine($"Method: {context.Request.Method}, Path: {context.Request.Path}, Headers: {context.Request.Headers}");
    
    await next();

    // Log response
    Console.WriteLine("Response:");
    Console.WriteLine($"StatusCode: {context.Response.StatusCode}");
});

// Add the custom middleware to log request and response
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Configure middleware
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Run the application
app.Run("http://0.0.0.0:8080");  // Use this line to specify port 8080
