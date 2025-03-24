using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.Data;
using TrelloClone.Models;

namespace TrelloClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ListsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/cards")]
        public async Task<ActionResult<IEnumerable<Card>>> GetListCards(int id)
        {
            var list = await _context.Lists
                .Include(l => l.Cards)
                .FirstOrDefaultAsync(l => l.Id == id);
                
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list.Cards);
        }

        [HttpPost("{id}/cards")]
        public async Task<ActionResult<Card>> CreateCard(int id, Card card)
        {
            var list = await _context.Lists.FindAsync(id);
            
            if (list == null)
            {
                return NotFound();
            }

            card.ListId = id;
            
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetCard", "Cards", new { id = card.Id }, card);
        }
    }
}
