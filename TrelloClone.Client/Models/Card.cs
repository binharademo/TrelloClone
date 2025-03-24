using System;
using System.ComponentModel.DataAnnotations;

namespace TrelloClone.Client.Models
{
    public class Card
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Card title is required")]
        [StringLength(200, ErrorMessage = "Card title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int ListId { get; set; }

        public int Position { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
