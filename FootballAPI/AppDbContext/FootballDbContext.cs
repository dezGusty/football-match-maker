using Microsoft.EntityFrameworkCore;
using FootballAPI.Models;
using FootballAPI.Models.Enums;

namespace FootballAPI.Data
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext(DbContextOptions<FootballDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchTeams> MatchTeams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        public DbSet<PlayerOrganiser> PlayerOrganisers { get; set; }
        public DbSet<OrganizerDelegate> OrganizerDelegates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProfileImagePath).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });


            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MatchDate).IsRequired();
                entity.Property(e => e.IsPublic).HasDefaultValue(false);
                entity.Property(e => e.Status).HasDefaultValue(Status.Open);
                entity.Property(e => e.Location).HasMaxLength(255);
                entity.Property(e => e.Cost).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Organiser)
                    .WithMany(u => u.OrganisedMatches)
                    .HasForeignKey(e => e.OrganiserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

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

            modelBuilder.Entity<TeamPlayers>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired();

                entity.HasOne(e => e.MatchTeam)
                    .WithMany(mt => mt.TeamPlayers)
                    .HasForeignKey(e => e.MatchTeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.TeamPlayers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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

            modelBuilder.Entity<PlayerOrganiser>(entity =>
            {
                entity.HasKey(e => new { e.OrganiserId, e.PlayerId });
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Organiser)
                    .WithMany(u => u.OrganisedPlayers)
                    .HasForeignKey(e => e.OrganiserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Player)
                    .WithMany(u => u.PlayerRelations)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrganizerDelegate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.OriginalOrganizer)
                    .WithMany(u => u.OriginalDelegations)
                    .HasForeignKey(e => e.OriginalOrganizerId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DelegateUser)
                    .WithMany(u => u.ReceivedDelegations)
                    .HasForeignKey(e => e.DelegateUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.OriginalOrganizerId, e.IsActive })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
            });

            SeedData(modelBuilder);

        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasData(
                new Team { Id = 1, Name = "FC Brasov" },
                new Team { Id = 2, Name = "Steaua Bucuresti" }
            );


            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "ion.popescu@gmail.com",
                    Username = "IonPopescu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Ion",
                    LastName = "Popescu",
                    Rating = 8.5f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Email = "marius.ionescu@gmail.com",
                    Username = "MariusIonescu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Marius",
                    LastName = "Ionescu",
                    Rating = 7.8f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 3,
                    Email = "admin@gmail.com",
                    Username = "Admin",
                    Password = "default123",
                    Role = UserRole.ADMIN,
                    FirstName = "Admin",
                    LastName = "User",
                    Rating = 0.0f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 4,
                    Email = "organiser@gmail.com",
                    Username = "Organiser",
                    Password = "default123",
                    Role = UserRole.ORGANISER,
                    FirstName = "Organiser",
                    LastName = "User",
                    Rating = 0.0f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 5,
                    Email = "alex.georgescu@gmail.com",
                    Username = "AlexGeorgescu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Alex",
                    LastName = "Georgescu",
                    Rating = 7.2f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 6,
                    Email = "razvan.moldovan@gmail.com",
                    Username = "RazvanMoldovan",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Razvan",
                    LastName = "Moldovan",
                    Rating = 8.1f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 7,
                    Email = "cristian.stancu@gmail.com",
                    Username = "CristianStancu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Cristian",
                    LastName = "Stancu",
                    Rating = 6.9f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 8,
                    Email = "andrei.vasilescu@gmail.com",
                    Username = "AndreiVasilescu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Andrei",
                    LastName = "Vasilescu",
                    Rating = 7.7f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 9,
                    Email = "florin.dumitru@gmail.com",
                    Username = "FlorinDumitru",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Florin",
                    LastName = "Dumitru",
                    Rating = 8.3f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 10,
                    Email = "gabriel.ciobanu@gmail.com",
                    Username = "GabrielCiobanu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Gabriel",
                    LastName = "Ciobanu",
                    Rating = 7.4f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 11,
                    Email = "lucian.matei@gmail.com",
                    Username = "LucianMatei",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Lucian",
                    LastName = "Matei",
                    Rating = 6.8f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 12,
                    Email = "daniel.radu@gmail.com",
                    Username = "DanielRadu",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Daniel",
                    LastName = "Radu",
                    Rating = 7.9f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 13,
                    Email = "mihai.popa@gmail.com",
                    Username = "MihaiPopa",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Mihai",
                    LastName = "Popa",
                    Rating = 8.0f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 14,
                    Email = "stefan.nicolae@gmail.com",
                    Username = "StefanNicolae",
                    Password = "default123",
                    Role = UserRole.PLAYER,
                    FirstName = "Stefan",
                    LastName = "Nicolae",
                    Rating = 7.6f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<PlayerOrganiser>().HasData(
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 1, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 2, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 5, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 6, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 7, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 8, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 9, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 10, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 11, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 12, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 13, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PlayerOrganiser { OrganiserId = 4, PlayerId = 14, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<FriendRequest>().HasData(
                new FriendRequest { Id = 1, SenderId = 4, ReceiverId = 1, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 2, SenderId = 4, ReceiverId = 2, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 3, SenderId = 4, ReceiverId = 5, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 4, SenderId = 4, ReceiverId = 6, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 5, SenderId = 4, ReceiverId = 7, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 6, SenderId = 4, ReceiverId = 8, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 7, SenderId = 4, ReceiverId = 9, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 8, SenderId = 4, ReceiverId = 10, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 9, SenderId = 4, ReceiverId = 11, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 10, SenderId = 4, ReceiverId = 12, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 11, SenderId = 4, ReceiverId = 13, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new FriendRequest { Id = 12, SenderId = 4, ReceiverId = 14, Status = FriendRequestStatus.Accepted, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), ResponsedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

    }
}