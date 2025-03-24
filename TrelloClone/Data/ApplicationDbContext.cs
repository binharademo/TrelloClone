using Microsoft.EntityFrameworkCore;
using TrelloClone.Models;

namespace TrelloClone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<List> Lists { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<CardHistory> CardHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Board>()
                .HasOne(b => b.User)
                .WithMany(u => u.Boards)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<List>()
                .HasOne(l => l.Board)
                .WithMany(b => b.Lists)
                .HasForeignKey(l => l.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Card>()
                .HasOne(c => c.List)
                .WithMany(l => l.Cards)
                .HasForeignKey(c => c.ListId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CardHistory>()
                .HasOne(ch => ch.Card)
                .WithMany(c => c.History)
                .HasForeignKey(ch => ch.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CardHistory>()
                .HasOne(ch => ch.User)
                .WithMany(u => u.CardHistories)
                .HasForeignKey(ch => ch.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
