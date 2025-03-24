using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Models;
using TrelloClone.Services;

namespace TrelloClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardsController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            
            var boards = await _boardService.GetUserBoardsAsync(userId.Value);
            return Ok(boards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetBoard(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            
            var board = await _boardService.GetBoardByIdAsync(id, userId.Value);
            
            if (board == null)
            {
                return NotFound();
            }

            return Ok(board);
        }

        [HttpPost]
        public async Task<ActionResult<Board>> CreateBoard(Board board)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            
            board.UserId = userId.Value;
            var createdBoard = await _boardService.CreateBoardAsync(board);
            
            return CreatedAtAction(nameof(GetBoard), new { id = createdBoard.Id }, createdBoard);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            
            var result = await _boardService.DeleteBoardAsync(id, userId.Value);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return null;
            }
            
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            
            return null;
        }
    }
}
