using System.Collections.Generic;

namespace TrelloClone.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        // Navigation properties
        public ICollection<Board> Boards { get; set; } = new List<Board>();
        public ICollection<CardHistory> CardHistories { get; set; } = new List<CardHistory>();
    }
}
