using System.Collections.Generic;

namespace TrelloClone.Models
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BoardId { get; set; }
        
        // Navigation properties
        public Board? Board { get; set; }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}
