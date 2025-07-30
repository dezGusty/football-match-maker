using Microsoft.EntityFrameworkCore;
using FootballAPI.Models;

namespace FootballAPI.Data
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext(DbContextOptions<FootballDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PlayerMatchHistory> PlayerMatchHistory { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.CurrentTeam)
                .WithMany(t => t.CurrentPlayers)
                .HasForeignKey(p => p.CurrentTeamId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.TeamA)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.TeamAId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.TeamB)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.TeamBId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerMatchHistory>()
                .HasOne(pmh => pmh.Player)
                .WithMany(p => p.MatchHistory)
                .HasForeignKey(pmh => pmh.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerMatchHistory>()
                .HasOne(pmh => pmh.Team)
                .WithMany(t => t.PlayerHistory)
                .HasForeignKey(pmh => pmh.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerMatchHistory>()
                .HasOne(pmh => pmh.Match)
                .WithMany(m => m.PlayerHistory)
                .HasForeignKey(pmh => pmh.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasIndex(p => new { p.FirstName, p.LastName });

            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Name);

            modelBuilder.Entity<Match>()
                .HasIndex(m => m.MatchDate);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Player>()
                .Property(p => p.Rating)
                .HasColumnType("float");

            modelBuilder.Entity<PlayerMatchHistory>()
                .Property(pmh => pmh.PerformanceRating)
                .HasColumnType("float");

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasData(
                new Team { Id = 1, Name = "FC Brasov" },
                new Team { Id = 2, Name = "Steaua Bucuresti" }
            );

            modelBuilder.Entity<Player>().HasData(
                new Player
                {
                    Id = 1,
                    FirstName = "Ion",
                    LastName = "Popescu",
                    Rating = 8.5f,
                    IsAvailable = true,
                    CurrentTeamId = 1
                },
                new Player
                {
                    Id = 2,
                    FirstName = "Marius",
                    LastName = "Ionescu",
                    Rating = 7.8f,
                    IsAvailable = true,
                    CurrentTeamId = 1
                }
            );


            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "parola123", Role = "Admin", Email="admin@gmail.com", ImageUrl = "http://localhost:5145/images/admin.jpg" }
            );
        }
    }
}
