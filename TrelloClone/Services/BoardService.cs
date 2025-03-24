using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrelloClone.Data;
using TrelloClone.Models;

namespace TrelloClone.Services
{
    public interface IBoardService
    {
        Task<IEnumerable<Board>> GetUserBoardsAsync(int userId);
        Task<Board?> GetBoardByIdAsync(int boardId, int userId);
        Task<Board> CreateBoardAsync(Board board);
        Task<bool> DeleteBoardAsync(int boardId, int userId);
    }

    public class BoardService : IBoardService
    {
        private readonly ApplicationDbContext _context;

        public BoardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Board>> GetUserBoardsAsync(int userId)
        {
            return await _context.Boards
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Board?> GetBoardByIdAsync(int boardId, int userId)
        {
            return await _context.Boards
                .Include(b => b.Lists)
                    .ThenInclude(l => l.Cards)
                .FirstOrDefaultAsync(b => b.Id == boardId && b.UserId == userId);
        }

        public async Task<Board> CreateBoardAsync(Board board)
        {
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            // Create the four fixed lists for the board
            var lists = new List<List>
            {
                new List { Name = "Backlog", BoardId = board.Id },
                new List { Name = "Priorized", BoardId = board.Id },
                new List { Name = "Doing", BoardId = board.Id },
                new List { Name = "Done", BoardId = board.Id }
            };

            _context.Lists.AddRange(lists);
            await _context.SaveChangesAsync();

            return board;
        }

        public async Task<bool> DeleteBoardAsync(int boardId, int userId)
        {
            var board = await _context.Boards
                .FirstOrDefaultAsync(b => b.Id == boardId && b.UserId == userId);

            if (board == null)
            {
                return false;
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
