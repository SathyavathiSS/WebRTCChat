using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebRTCService.Controllers;
using WebRTCService.Hubs;
using WebRTCService.Services;
using WebRTCService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://refactored-disco-r79rx4x5gwrfxvjr-8082.app.github.dev")
                            .AllowAnyHeader()
                            .AllowAnyMethod());
});

// Register IChatService as scoped
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddSwaggerGen();
// Check if the connection string exists in the configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the hub
builder.Services.AddScoped<ChatHub>();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebRTCService API V1");
    });
    
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

//app.MapHub<ChatHub>("/chatHub");
app.MapHub<ChatHub>("/ws");

// Map API routes
app.MapControllers();

app.Run();
