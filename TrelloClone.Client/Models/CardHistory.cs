using System;

namespace TrelloClone.Client.Models
{
    public class CardHistory
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
