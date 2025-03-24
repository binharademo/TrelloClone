using System.Collections.Generic;

namespace TrelloClone.Client.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Navigation properties
        public List<Board>? Boards { get; set; }
        public List<CardHistory>? CardHistories { get; set; }
    }
}
