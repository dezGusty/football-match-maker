using Microsoft.EntityFrameworkCore;
using FootballAPI.Models;

namespace FootballAPI.Data
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext(DbContextOptions<FootballDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchTeams> MatchTeams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        public DbSet<PlayerOrganiser> PlayerOrganisers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Player configurations
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProfileImagePath).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Player>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Team configurations
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Match configurations
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MatchDate).IsRequired();
                entity.Property(e => e.IsPublic).HasDefaultValue(false);
                entity.Property(e => e.Status).HasDefaultValue(Status.Open);
            });

            // MatchTeams configurations (Many-to-Many between Match and Team)
            modelBuilder.Entity<MatchTeams>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Goals).HasDefaultValue(0);
                
                entity.HasOne(e => e.Match)
                    .WithMany(m => m.MatchTeams)
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Team)
                    .WithMany(t => t.MatchTeams)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TeamPlayers configurations (Many-to-Many between Team and Player through MatchTeam)
            modelBuilder.Entity<TeamPlayers>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired();
                
                entity.HasOne(e => e.MatchTeam)
                    .WithMany(mt => mt.TeamPlayers)
                    .HasForeignKey(e => e.MatchTeamId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.TeamPlayers)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // FriendRequest configurations
            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue(FriendRequestStatus.Pending);
                entity.Property(e => e.CreatedAt).HasDefaultValue(DateTime.Now);
                
                entity.HasOne(e => e.Sender)
                    .WithMany(u => u.SentFriendRequests)
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Receiver)
                    .WithMany(u => u.ReceivedFriendRequests)
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ResetPasswordToken configurations
            modelBuilder.Entity<ResetPasswordToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.ResetPasswordTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PlayerOrganiser configurations (Many-to-Many between User as Organiser and Player)
            modelBuilder.Entity<PlayerOrganiser>(entity =>
            {
                entity.HasKey(e => new { e.OrganiserId, e.PlayerId });
                entity.Property(e => e.CreatedAt).HasDefaultValue(DateTime.Now);
                
                entity.HasOne(e => e.Organiser)
                    .WithMany(u => u.OrganisedPlayers)
                    .HasForeignKey(e => e.OrganiserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerOrganisers)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}