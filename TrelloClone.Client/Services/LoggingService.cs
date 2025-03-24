using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TrelloClone.Client.Services
{
    public interface ILoggingService
    {
        Task LogInformation(string message);
        Task LogWarning(string message);
        Task LogError(string message);
        Task LogDebug(string message);
    }

    public class LoggingService : ILoggingService
    {
        private readonly IJSRuntime _jsRuntime;

        public LoggingService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task LogInformation(string message)
        {
            await _jsRuntime.InvokeVoidAsync("console.info", message);
        }

        public async Task LogWarning(string message)
        {
            await _jsRuntime.InvokeVoidAsync("console.warn", message);
        }

        public async Task LogError(string message)
        {
            await _jsRuntime.InvokeVoidAsync("console.error", message);
        }

        public async Task LogDebug(string message)
        {
            await _jsRuntime.InvokeVoidAsync("console.debug", message);
        }
    }
}
