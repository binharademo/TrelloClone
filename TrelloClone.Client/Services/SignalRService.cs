using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using TrelloClone.Client.Models;

namespace TrelloClone.Client.Services
{
    public interface ISignalRService
    {
        event Action<Card, string> CardAdded;
        event Action<Card, string> CardUpdated;
        event Action<Card, int, int, string> CardMoved;
        event Action<int, string> CardDeleted;
        
        Task StartConnection(int boardId);
        Task StopConnection();
    }

    public class SignalRService : ISignalRService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAuthService _authService;
        private HubConnection? _hubConnection;
        
        public event Action<Card, string>? CardAdded;
        public event Action<Card, string>? CardUpdated;
        public event Action<Card, int, int, string>? CardMoved;
        public event Action<int, string>? CardDeleted;

        public SignalRService(NavigationManager navigationManager, IAuthService authService)
        {
            _navigationManager = navigationManager;
            _authService = authService;
        }

        public async Task StartConnection(int boardId)
        {
            if (_hubConnection != null)
            {
                return;
            }

            var token = await _authService.GetToken();
            
            // Create the connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri($"/boardhub?boardId={boardId}"), options =>
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    }
                })
                .WithAutomaticReconnect()
                .Build();

            // Set up event handlers
            _hubConnection.On<Card, string>("CardAdded", (card, username) =>
            {
                CardAdded?.Invoke(card, username);
            });

            _hubConnection.On<Card, string>("CardUpdated", (card, username) =>
            {
                CardUpdated?.Invoke(card, username);
            });

            _hubConnection.On<Card, int, int, string>("CardMoved", (card, fromListId, toListId, username) =>
            {
                CardMoved?.Invoke(card, fromListId, toListId, username);
            });

            _hubConnection.On<int, string>("CardDeleted", (cardId, username) =>
            {
                CardDeleted?.Invoke(cardId, username);
            });

            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR Connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR Connection Error: {ex.Message}");
            }
        }

        public async Task StopConnection()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                Console.WriteLine("SignalR Disconnected");
            }
        }
    }
}
