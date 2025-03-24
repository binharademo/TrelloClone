using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrelloClone.Client.Models;

namespace TrelloClone.Client.Services
{
    public interface IBoardService
    {
        Task<List<Board>> GetBoardsAsync();
        Task<Board?> GetBoardAsync(int boardId);
        Task<Board?> CreateBoardAsync(Board board);
        Task<bool> DeleteBoardAsync(int boardId);
        Task<List?> AddListAsync(List list);
        Task<bool> DeleteListAsync(int listId);
    }

    public class BoardService : IBoardService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public BoardService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<Board>> GetBoardsAsync()
        {
            await SetAuthHeader();
            try
            {
                var boards = await _httpClient.GetFromJsonAsync<List<Board>>("api/boards");
                return boards ?? new List<Board>();
            }
            catch (Exception)
            {
                return new List<Board>();
            }
        }

        public async Task<Board?> GetBoardAsync(int boardId)
        {
            await SetAuthHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<Board>($"api/boards/{boardId}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Board?> CreateBoardAsync(Board board)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/boards", board);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Board>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteBoardAsync(int boardId)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.DeleteAsync($"api/boards/{boardId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List?> AddListAsync(List list)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/lists", list);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteListAsync(int listId)
        {
            await SetAuthHeader();
            try
            {
                var response = await _httpClient.DeleteAsync($"api/lists/{listId}");
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
