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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Organiser)
                    .WithMany(u => u.OrganisedPlayers)
                    .HasForeignKey(e => e.OrganiserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerOrganisers)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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
                    UserId = 1,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 2,
                    FirstName = "Marius",
                    LastName = "Ionescu",
                    Rating = 7.8f,
                    UserId = 2,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 3,
                    FirstName = "Alex",
                    LastName = "Georgescu",
                    Rating = 7.2f,
                    UserId = 5,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 4,
                    FirstName = "Razvan",
                    LastName = "Moldovan",
                    Rating = 8.1f,
                    UserId = 6,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 5,
                    FirstName = "Cristian",
                    LastName = "Stancu",
                    Rating = 6.9f,
                    UserId = 7,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 6,
                    FirstName = "Andrei",
                    LastName = "Vasilescu",
                    Rating = 7.7f,
                    UserId = 8,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 7,
                    FirstName = "Florin",
                    LastName = "Dumitru",
                    Rating = 8.3f,
                    UserId = 9,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 8,
                    FirstName = "Gabriel",
                    LastName = "Ciobanu",
                    Rating = 7.4f,
                    UserId = 10,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 9,
                    FirstName = "Lucian",
                    LastName = "Matei",
                    Rating = 6.8f,
                    UserId = 11,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 10,
                    FirstName = "Daniel",
                    LastName = "Radu",
                    Rating = 7.9f,
                    UserId = 12,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 11,
                    FirstName = "Mihai",
                    LastName = "Popa",
                    Rating = 8.0f,
                    UserId = 13,
                    IsAvailable = true
                },
                new Player
                {
                    Id = 12,
                    FirstName = "Stefan",
                    LastName = "Nicolae",
                    Rating = 7.6f,
                    UserId = 14,
                    IsAvailable = true
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "ion.popescu@gmail.com",
                    Username = "IonPopescu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 2,
                    Email = "marius.ionescu@gmail.com",
                    Username = "MariusIonescu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 3,
                    Email = "admin@gmail.com",
                    Username = "Admin",
                    Password = "default123",
                    Role = UserRole.ADMIN
                },
                new User
                {
                    Id = 4,
                    Email = "organiser@gmail.com",
                    Username = "Organiser",
                    Password = "default123",
                    Role = UserRole.ORGANISER
                },
                new User
                {
                    Id = 5,
                    Email = "alex.georgescu@gmail.com",
                    Username = "AlexGeorgescu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 6,
                    Email = "razvan.moldovan@gmail.com",
                    Username = "RazvanMoldovan",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 7,
                    Email = "cristian.stancu@gmail.com",
                    Username = "CristianStancu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 8,
                    Email = "andrei.vasilescu@gmail.com",
                    Username = "AndreiVasilescu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 9,
                    Email = "florin.dumitru@gmail.com",
                    Username = "FlorinDumitru",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 10,
                    Email = "gabriel.ciobanu@gmail.com",
                    Username = "GabrielCiobanu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 11,
                    Email = "lucian.matei@gmail.com",
                    Username = "LucianMatei",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 12,
                    Email = "daniel.radu@gmail.com",
                    Username = "DanielRadu",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 13,
                    Email = "mihai.popa@gmail.com",
                    Username = "MihaiPopa",
                    Password = "default123",
                    Role = UserRole.PLAYER
                },
                new User
                {
                    Id = 14,
                    Email = "stefan.nicolae@gmail.com",
                    Username = "StefanNicolae",
                    Password = "default123",
                    Role = UserRole.PLAYER
                }


            );
        }

    }
}