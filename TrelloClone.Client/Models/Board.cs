using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrelloClone.Client.Models
{
    public class Board
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Board name is required")]
        [StringLength(100, ErrorMessage = "Board name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<List> Lists { get; set; } = new List<List>();
    }
}
