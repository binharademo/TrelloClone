using System;
using System.Collections.Generic;

namespace TrelloClone.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ListId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // Navigation properties
        public List? List { get; set; }
        public ICollection<CardHistory> History { get; set; } = new List<CardHistory>();
    }
}
