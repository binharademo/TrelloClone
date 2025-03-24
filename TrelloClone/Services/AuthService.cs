using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TrelloClone.Data;
using TrelloClone.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace TrelloClone.Services
{
    public interface IAuthService
    {
        Task<(AuthResponse? Response, string? ErrorMessage)> RegisterAsync(RegisterModel model);
        Task<(AuthResponse? Response, string? ErrorMessage)> LoginAsync(LoginModel model);
        string HashPassword(string password);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(AuthResponse? Response, string? ErrorMessage)> RegisterAsync(RegisterModel model)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", model.Email);

                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists", model.Email);
                    return (null, "User with this email already exists");
                }

                // Create new user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password)
                };

                _logger.LogInformation("Creating new user with email: {Email}, username: {Username}", 
                    user.Email, user.Username);
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate token
                var token = GenerateJwtToken(user);

                _logger.LogInformation("User registered successfully: {UserId}, {Email}", user.Id, user.Email);

                return (new AuthResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
                }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Message}", ex.Message);
                return (null, $"Registration failed: {ex.Message}");
            }
        }

        public async Task<(AuthResponse? Response, string? ErrorMessage)> LoginAsync(LoginModel model)
        {
            _logger.LogInformation($"Attempting login for user with email: {model.Email}");
            
            try
            {
                // Find user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User with email {model.Email} not found in database");
                    return (null, "Invalid email or password");
                }
                
                _logger.LogInformation($"User found: ID={user.Id}, Email={user.Email}, Username={user.Username}");
                
                // Verify password
                _logger.LogInformation($"Verifying password for user: {user.Email}");
                var passwordHash = HashPassword(model.Password);
                var storedHash = user.PasswordHash;
                
                _logger.LogDebug($"Password verification - Input hash: {passwordHash.Substring(0, 10)}..., Stored hash: {storedHash.Substring(0, 10)}...");
                
                if (passwordHash != storedHash)
                {
                    _logger.LogWarning($"Login failed: Invalid password for user {model.Email}");
                    _logger.LogDebug($"Password hashes don't match. Input: {passwordHash}, Stored: {storedHash}");
                    return (null, "Invalid email or password");
                }
                
                _logger.LogInformation($"Password verified successfully for user: {user.Email}");
                
                // Generate JWT token
                var token = GenerateJwtToken(user);
                _logger.LogInformation($"JWT token generated for user: {user.Email}");
                
                // Return successful response
                return (new AuthResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
                }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception during login for user {model.Email}: {ex.Message}");
                return (null, $"An error occurred: {ex.Message}");
            }
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            var result = hashedPassword == storedHash;
            _logger.LogInformation("Password verification result: {Result}", result);
            return result;
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var jwtKey = _configuration["JwtSettings:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT Key not configured");
                    throw new InvalidOperationException("JWT Key not configured");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
                var expiryDays = _configuration["JwtSettings:ExpiryInDays"] ?? "7";
                var expires = DateTime.Now.AddDays(Convert.ToDouble(expiryDays));

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token: {Message}", ex.Message);
                throw;
            }
        }
    }
}
