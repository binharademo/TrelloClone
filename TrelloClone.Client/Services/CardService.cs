using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrelloClone.Client.Models;

namespace TrelloClone.Client.Services
{
    public interface ICardService
    {
        Task<Card?> GetCardAsync(int cardId);
        Task<List<CardHistory>> GetCardHistoryAsync(int cardId);
        Task<Card?> CreateCardAsync(Card card);
        Task<Card?> UpdateCardAsync(Card card);
        Task<Card?> MoveCardAsync(int cardId, int targetListId, int position);
        Task<bool> DeleteCardAsync(int cardId);
    }

    public class CardService : ICardService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public CardService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<Card?> GetCardAsync(int cardId)
        {
            await SetAuthHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<Card>($"api/cards/{cardId}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<CardHistory>> GetCardHistoryAsync(int cardId)
        {
            await SetAuthHeader();
            try
            {
                var history = await _httpClient.GetFromJsonAsync<List<CardHistory>>($"api/cards/{cardId}/history");
                return history ?? new List<CardHistory>();
            }
            catch (Exception)
            {
                return new List<CardHistory>();
            }
        }

        public async Task<Card?> CreateCardAsync(Card card)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cards", card);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Card>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Card?> UpdateCardAsync(Card card)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/cards/{card.Id}", card);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Card>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Card?> MoveCardAsync(int cardId, int targetListId, int position)
        {
            await SetAuthHeader();
            try
            {
                var moveData = new { ListId = targetListId, Position = position };
                var response = await _httpClient.PutAsJsonAsync($"api/cards/{cardId}/move", moveData);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Card>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteCardAsync(int cardId)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.DeleteAsync($"api/cards/{cardId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task SetAuthHeader()
        {
            var token = await _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
