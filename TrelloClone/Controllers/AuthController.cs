using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Models;
using TrelloClone.Services;
using Microsoft.Extensions.Logging;

namespace TrelloClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            _logger.LogInformation("Registration request received for email: {Email}", model.Email);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for registration request");
                return BadRequest(new { message = "Invalid registration data", errors = ModelState });
            }

            var (response, errorMessage) = await _authService.RegisterAsync(model);
            
            if (response == null)
            {
                _logger.LogWarning("Registration failed: {ErrorMessage}", errorMessage);
                return BadRequest(new { message = errorMessage ?? "Registration failed" });
            }

            _logger.LogInformation("User registered successfully: {UserId}, {Email}", response.UserId, response.Email);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            _logger.LogInformation($"Login attempt for email: {model.Email} at {DateTime.Now}");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid model state for login attempt with email: {model.Email}");
                return BadRequest(ModelState);
            }

            var (response, errorMessage) = await _authService.LoginAsync(model);
            
            if (response != null)
            {
                _logger.LogInformation($"Login successful for email: {model.Email}");
                return Ok(response);
            }
            
            _logger.LogWarning($"Login failed for email: {model.Email}. Error: {errorMessage}");
            return BadRequest(new { message = errorMessage });
        }

        [HttpGet("hash")]
        public IActionResult GetPasswordHash([FromQuery] string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password is required");
            }
            
            _logger.LogInformation("Password hash requested");
            var hash = _authService.HashPassword(password);
            return Ok(hash);
        }
    }
}
