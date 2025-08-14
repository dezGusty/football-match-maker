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
        public DbSet<PlayerOrganiser> PlayerOrganisers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<PlayerOrganiser>()
                .HasOne(po => po.Organiser)
                .WithMany()
                .HasForeignKey(po => po.OrganiserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerOrganiser>()
                .HasOne(po => po.Player)
                .WithMany()
                .HasForeignKey(po => po.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerOrganiser>()
                .HasKey(po => new { po.OrganiserId, po.PlayerId });

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany()
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .Property(fr => fr.Status)
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.Entity<FriendRequest>()
                .HasIndex(fr => new { fr.SenderId, fr.ReceiverId })
                .IsUnique();

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

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.Email)
                .HasPrincipalKey(u => u.Email)
                .OnDelete(DeleteBehavior.Restrict);

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
                    Email = "ion.popescu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 2,
                    FirstName = "Marius",
                    LastName = "Ionescu",
                    Rating = 7.8f,
                    Email = "marius.ionescu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 3,
                    FirstName = "Alex",
                    LastName = "Georgescu",
                    Rating = 7.2f,
                    Email = "alex.georgescu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 4,
                    FirstName = "Razvan",
                    LastName = "Moldovan",
                    Rating = 8.1f,
                    Email = "razvan.moldovan@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 5,
                    FirstName = "Cristian",
                    LastName = "Stancu",
                    Rating = 6.9f,
                    Email = "cristian.stancu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 6,
                    FirstName = "Andrei",
                    LastName = "Vasilescu",
                    Rating = 7.7f,
                    Email = "andrei.vasilescu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 7,
                    FirstName = "Florin",
                    LastName = "Dumitru",
                    Rating = 8.3f,
                    Email = "florin.dumitru@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 8,
                    FirstName = "Gabriel",
                    LastName = "Ciobanu",
                    Rating = 7.4f,
                    Email = "gabriel.ciobanu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 9,
                    FirstName = "Lucian",
                    LastName = "Matei",
                    Rating = 6.8f,
                    Email = "lucian.matei@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 10,
                    FirstName = "Daniel",
                    LastName = "Radu",
                    Rating = 7.9f,
                    Email = "daniel.radu@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 11,
                    FirstName = "Mihai",
                    LastName = "Popa",
                    Rating = 8.0f,
                    Email = "mihai.popa@gmail.com",
                    IsAvailable = true
                },
                new Player
                {
                    Id = 12,
                    FirstName = "Stefan",
                    LastName = "Nicolae",
                    Rating = 7.6f,
                    Email = "stefan.nicolae@gmail.com",
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
