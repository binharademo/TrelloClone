using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using TrelloClone.Client.Models;
using System.Text.Json;
using System.Text;
using System.Diagnostics;

namespace TrelloClone.Client.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> Register(RegisterModel model);
        Task<(bool Success, string? ErrorMessage)> Login(LoginModel model);
        Task<bool> IsAuthenticated();
        Task<string?> GetToken();
        Task Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILoggingService _logger;
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, ILoggingService logger)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async Task<(bool Success, string? ErrorMessage)> Register(RegisterModel model)
        {
            try
            {
                await _logger.LogInformation($"Registering user with email: {model.Email}");
                
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                await _logger.LogInformation($"Register response status: {response.StatusCode}");
                await _logger.LogDebug($"Register response content: {responseContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<AuthResult>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserIdKey, result.UserId.ToString());
                        await _logger.LogInformation($"User registered successfully: {result.UserId}, {result.Email}");
                        return (true, null);
                    }
                }
                
                // Try to extract error message from response if possible
                string errorMessage = "Registration failed";
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                    {
                        errorMessage = errorResponse.Message;
                    }
                }
                catch (JsonException jsonEx)
                {
                    await _logger.LogWarning($"JSON parsing error: {jsonEx.Message}");
                }
                
                await _logger.LogWarning($"Registration failed: {errorMessage}");
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Registration exception: {ex.Message}\nStack trace: {ex.StackTrace}");
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> Login(LoginModel model)
        {
            try
            {
                await _logger.LogInformation($"Login attempt for email: {model.Email}");
                
                // Use URL absoluta para evitar problemas com o proxy do browser preview
                var apiUrl = "http://localhost:5022/api/auth/login";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, model);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                await _logger.LogInformation($"Login response status: {response.StatusCode}");
                await _logger.LogDebug($"Login response content: {responseContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<AuthResult>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (result != null && !string.IsNullOrEmpty(result.Token))
                        {
                            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
                            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserIdKey, result.UserId.ToString());
                            await _logger.LogInformation($"User logged in successfully: {result.UserId}, {result.Email}");
                            return (true, null);
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        await _logger.LogWarning($"JSON parsing error: {jsonEx.Message}");
                        return (false, $"Failed to parse server response: {jsonEx.Message}");
                    }
                }
                
                // Try to extract error message from response if possible
                string errorMessage = "Invalid email or password";
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                    {
                        errorMessage = errorResponse.Message;
                    }
                }
                catch (JsonException)
                {
                    // If we can't parse the error response, just use the default message
                }
                
                await _logger.LogWarning($"Login failed for email: {model.Email}. Status: {response.StatusCode}, Error: {errorMessage}");
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Login exception: {ex.Message}\nStack trace: {ex.StackTrace}");
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await GetToken();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetToken()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
                await _logger.LogDebug($"GetToken: {(token != null ? "Token found" : "No token found")}");
                return token;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"GetToken exception: {ex.Message}");
                return null;
            }
        }

        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserIdKey);
            await _logger.LogInformation("User logged out");
        }
    }

    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
