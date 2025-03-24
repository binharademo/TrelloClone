using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TrelloClone.Data;
using TrelloClone.Services;
using TrelloClone.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure logging
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "Logs");
if (!Directory.Exists(logsDirectory))
{
    Directory.CreateDirectory(logsDirectory);
}

// Configure logging
builder.Logging.AddFile(Path.Combine(logsDirectory, "api_logs.txt"));

// Configure SQLite database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=trelloclone.db");
    options.EnableSensitiveDataLogging(); // Enable detailed SQL logging
    options.LogTo(message =>
    {
        if (message.Contains("CommandExecuted") || message.Contains("Executing DbCommand"))
        {
            string logMessage = $"[{DateTime.Now}] [SQL] {message}{Environment.NewLine}";
            File.AppendAllText(Path.Combine(logsDirectory, "sql_logs.txt"), logMessage);
        }
    }, LogLevel.Information);
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<ICardService, CardService>();

// Add SignalR
builder.Services.AddSignalR();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // Configure SignalR to use JWT authentication
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers and SignalR hub
app.MapControllers();
app.MapHub<BoardHub>("/hubs/board");

// Add request logging middleware
app.Use(async (context, next) =>
{
    var requestId = Guid.NewGuid().ToString();
    var requestTime = DateTime.Now;
    var requestMethod = context.Request.Method;
    var requestPath = context.Request.Path;
    var requestQuery = context.Request.QueryString;
    
    // Log the request
    string requestLogMessage = $"[{requestTime}] [REQUEST {requestId}] {requestMethod} {requestPath}{requestQuery}{Environment.NewLine}";
    File.AppendAllText(Path.Combine(logsDirectory, "api_logs.txt"), requestLogMessage);
    
    // Capture the original body stream
    var originalBodyStream = context.Response.Body;
    
    try
    {
        // Create a new memory stream to capture the response
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        // Continue down the pipeline
        await next.Invoke();
        
        // Read the response
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        
        // Log the response
        var responseTime = DateTime.Now;
        var statusCode = context.Response.StatusCode;
        var duration = (responseTime - requestTime).TotalMilliseconds;
        
        string responseLogMessage = $"[{responseTime}] [RESPONSE {requestId}] {statusCode} ({duration}ms){Environment.NewLine}";
        if (!string.IsNullOrEmpty(responseContent) && responseContent.Length < 1000) // Limit large responses
        {
            responseLogMessage += $"Response Body: {responseContent}{Environment.NewLine}";
        }
        else
        {
            responseLogMessage += $"Response Body: [Content too large to log]{Environment.NewLine}";
        }
        
        File.AppendAllText(Path.Combine(logsDirectory, "api_logs.txt"), responseLogMessage);
        
        // Copy the response to the original stream and return
        await responseBody.CopyToAsync(originalBodyStream);
    }
    finally
    {
        // Restore the original stream
        context.Response.Body = originalBodyStream;
    }
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var authService = services.GetRequiredService<IAuthService>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    dbContext.Database.EnsureCreated();
    
    // Check if we need to seed the database with a test user
    if (!dbContext.Users.Any())
    {
        logger.LogInformation("Seeding database with test user...");
        
        // Create test user
        var testUser = new TrelloClone.Models.User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = authService.HashPassword("password123")
        };
        
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();
        
        logger.LogInformation("Test user created successfully with ID: {UserId}", testUser.Id);
    }
}

app.Run();
