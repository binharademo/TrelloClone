using System;

namespace TrelloClone.Models
{
    public class CardHistory
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int FromListId { get; set; }
        public int ToListId { get; set; }
        public DateTime MovedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        
        // Navigation properties
        public Card? Card { get; set; }
        public User? User { get; set; }
    }
}
