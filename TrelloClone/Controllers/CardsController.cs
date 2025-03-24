using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TrelloClone.Hubs;
using TrelloClone.Models;
using TrelloClone.Services;

namespace TrelloClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IHubContext<BoardHub> _boardHub;

        public CardsController(ICardService cardService, IHubContext<BoardHub> boardHub)
        {
            _cardService = cardService;
            _boardHub = boardHub;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            
            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<CardHistory>>> GetCardHistory(int id)
        {
            var history = await _cardService.GetCardHistoryAsync(id);
            return Ok(history);
        }

        [HttpPost]
        public async Task<ActionResult<Card>> CreateCard(Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCard = await _cardService.CreateCardAsync(card);
            
            // Notify clients about the new card
            await _boardHub.Clients.Group($"board-{createdCard.List?.BoardId}")
                .SendAsync("ReceiveCardAdded", createdCard, User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous");
            
            return CreatedAtAction(nameof(GetCard), new { id = createdCard.Id }, createdCard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, Card card)
        {
            if (id != card.Id)
            {
                return BadRequest();
            }

            var updatedCard = await _cardService.UpdateCardAsync(card);
            
            if (updatedCard == null)
            {
                return NotFound();
            }

            // Notify clients about the updated card
            await _boardHub.Clients.Group($"board-{updatedCard.List?.BoardId}")
                .SendAsync("ReceiveCardUpdated", updatedCard, User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous");
            
            return NoContent();
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveCard(int id, [FromBody] MoveCardRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            
            var userId = int.Parse(userIdClaim);
            var card = await _cardService.MoveCardAsync(id, request.ToListId, userId);
            
            if (card == null)
            {
                return NotFound();
            }

            // Notify clients about the moved card
            await _boardHub.Clients.Group($"board-{card.List?.BoardId}")
                .SendAsync("ReceiveCardMoved", card, request.FromListId, request.ToListId, User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous");
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            var boardId = card.List?.BoardId;
            var result = await _cardService.DeleteCardAsync(id);
            
            if (!result)
            {
                return NotFound();
            }

            // Notify clients about the deleted card
            if (boardId.HasValue)
            {
                await _boardHub.Clients.Group($"board-{boardId}")
                    .SendAsync("ReceiveCardDeleted", id, User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous");
            }
            
            return NoContent();
        }
    }

    public class MoveCardRequest
    {
        public int FromListId { get; set; }
        public int ToListId { get; set; }
    }
}
