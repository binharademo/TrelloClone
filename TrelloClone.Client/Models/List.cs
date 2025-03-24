using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrelloClone.Client.Models
{
    public class List
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "List name is required")]
        [StringLength(100, ErrorMessage = "List name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public int Position { get; set; }

        public int BoardId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
