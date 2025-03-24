using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TrelloClone.Models;

namespace TrelloClone.Hubs
{
    public class BoardHub : Hub
    {
        public async Task JoinBoard(int boardId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetBoardGroupName(boardId));
        }

        public async Task LeaveBoard(int boardId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetBoardGroupName(boardId));
        }

        public async Task CardMoved(int boardId, Card card, int fromListId, int toListId, string username)
        {
            await Clients.Group(GetBoardGroupName(boardId)).SendAsync("ReceiveCardMoved", card, fromListId, toListId, username);
        }

        public async Task CardUpdated(int boardId, Card card, string username)
        {
            await Clients.Group(GetBoardGroupName(boardId)).SendAsync("ReceiveCardUpdated", card, username);
        }

        public async Task CardAdded(int boardId, Card card, string username)
        {
            await Clients.Group(GetBoardGroupName(boardId)).SendAsync("ReceiveCardAdded", card, username);
        }

        public async Task CardDeleted(int boardId, int cardId, string username)
        {
            await Clients.Group(GetBoardGroupName(boardId)).SendAsync("ReceiveCardDeleted", cardId, username);
        }

        private static string GetBoardGroupName(int boardId)
        {
            return $"board-{boardId}";
        }
    }
}
