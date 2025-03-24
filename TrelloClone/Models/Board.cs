using System.Collections.Generic;

namespace TrelloClone.Models
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserId { get; set; }
        
        // Navigation properties
        public User? User { get; set; }
        public ICollection<List> Lists { get; set; } = new List<List>();
    }
}
