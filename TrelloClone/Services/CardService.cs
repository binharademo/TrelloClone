using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrelloClone.Data;
using TrelloClone.Models;

namespace TrelloClone.Services
{
    public interface ICardService
    {
        Task<Card?> GetCardByIdAsync(int cardId);
        Task<Card> CreateCardAsync(Card card);
        Task<Card?> UpdateCardAsync(Card card);
        Task<Card?> MoveCardAsync(int cardId, int toListId, int userId);
        Task<bool> DeleteCardAsync(int cardId);
        Task<IEnumerable<CardHistory>> GetCardHistoryAsync(int cardId);
    }

    public class CardService : ICardService
    {
        private readonly ApplicationDbContext _context;

        public CardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Card?> GetCardByIdAsync(int cardId)
        {
            return await _context.Cards
                .Include(c => c.List)
                .FirstOrDefaultAsync(c => c.Id == cardId);
        }

        public async Task<Card> CreateCardAsync(Card card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<Card?> UpdateCardAsync(Card card)
        {
            var existingCard = await _context.Cards.FindAsync(card.Id);
            
            if (existingCard == null)
            {
                return null;
            }

            existingCard.Title = card.Title;
            existingCard.Description = card.Description;
            existingCard.DueDate = card.DueDate;
            
            // If the card is moved to the "Done" list, set the CompletedAt date
            var doneList = await _context.Lists
                .FirstOrDefaultAsync(l => l.BoardId == existingCard.List!.BoardId && l.Name == "Done");
                
            if (doneList != null && card.ListId == doneList.Id && existingCard.CompletedAt == null)
            {
                existingCard.CompletedAt = DateTime.UtcNow;
            }
            // If the card is moved from "Done" to another list, clear the CompletedAt date
            else if (doneList != null && card.ListId != doneList.Id)
            {
                existingCard.CompletedAt = null;
            }

            await _context.SaveChangesAsync();
            return existingCard;
        }

        public async Task<Card?> MoveCardAsync(int cardId, int toListId, int userId)
        {
            var card = await _context.Cards
                .Include(c => c.List)
                .FirstOrDefaultAsync(c => c.Id == cardId);
                
            if (card == null)
            {
                return null;
            }

            var toList = await _context.Lists.FindAsync(toListId);
            if (toList == null)
            {
                return null;
            }

            var fromListId = card.ListId;
            
            // Create history record
            var history = new CardHistory
            {
                CardId = cardId,
                FromListId = fromListId,
                ToListId = toListId,
                MovedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.CardHistories.Add(history);
            
            // Update card's list
            card.ListId = toListId;
            
            // If the card is moved to the "Done" list, set the CompletedAt date
            var doneList = await _context.Lists
                .FirstOrDefaultAsync(l => l.BoardId == toList.BoardId && l.Name == "Done");
                
            if (doneList != null && toListId == doneList.Id)
            {
                card.CompletedAt = DateTime.UtcNow;
            }
            // If the card is moved from "Done" to another list, clear the CompletedAt date
            else if (doneList != null && fromListId == doneList.Id && toListId != doneList.Id)
            {
                card.CompletedAt = null;
            }

            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<bool> DeleteCardAsync(int cardId)
        {
            var card = await _context.Cards.FindAsync(cardId);
            
            if (card == null)
            {
                return false;
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CardHistory>> GetCardHistoryAsync(int cardId)
        {
            return await _context.CardHistories
                .Include(ch => ch.User)
                .Where(ch => ch.CardId == cardId)
                .OrderByDescending(ch => ch.MovedAt)
                .ToListAsync();
        }
    }
}
