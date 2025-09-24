using Microsoft.EntityFrameworkCore;
using FootballAPI.Models;
using FootballAPI.Models.Enums;

namespace FootballAPI.Data
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext(DbContextOptions<FootballDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCredentials> UserCredentials { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchTeams> MatchTeams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        public DbSet<OrganizerDelegate> OrganizerDelegates { get; set; }
        public DbSet<MatchTemplate> MatchTemplates { get; set; }
        public DbSet<ImpersonationLog> ImpersonationLogs { get; set; }
        public DbSet<RatingHistory> RatingHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProfileImagePath).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.Username).IsUnique();

                // Configure one-to-one relationship with UserCredentials
                entity.HasOne(e => e.Credentials)
                    .WithOne(c => c.User)
                    .HasForeignKey<UserCredentials>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserCredentials>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
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
                entity.Property(e => e.Goals).HasDefaultValue(null);

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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

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

            modelBuilder.Entity<MatchTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Cost).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ImpersonationLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StartTime).IsRequired();

                // Configure relationships
                entity.HasOne(e => e.Admin)
                    .WithMany()
                    .HasForeignKey(e => e.AdminId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ImpersonatedUser)
                    .WithMany()
                    .HasForeignKey(e => e.ImpersonatedUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<RatingHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NewRating).IsRequired();
                entity.Property(e => e.ChangeReason).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RatingSystem).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Match)
                    .WithMany()
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.SetNull);
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
                    Username = "IonPopescu",
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
                    Username = "MariusIonescu",
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
                    Username = "Admin",
                    Role = UserRole.ADMIN,
                    FirstName = "Admin",
                    LastName = "User",
                    Rating = 5.0f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 4,
                    Username = "Organiser",
                    Role = UserRole.ORGANISER,
                    FirstName = "Organiser",
                    LastName = "User",
                    Rating = 5.0f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 5,
                    Username = "AlexGeorgescu",
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
                    Username = "RazvanMoldovan",
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
                    Username = "CristianStancu",
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
                    Username = "AndreiVasilescu",
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
                    Username = "FlorinDumitru",
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
                    Username = "GabrielCiobanu",
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
                    Username = "LucianMatei",
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
                    Username = "DanielRadu",
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
                    Username = "MihaiPopa",
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
                    Username = "StefanNicolae",
                    Role = UserRole.PLAYER,
                    FirstName = "Stefan",
                    LastName = "Nicolae",
                    Rating = 7.6f,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<UserCredentials>().HasData(
                new UserCredentials { Id = 1, UserId = 1, Email = "ion.popescu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 2, UserId = 2, Email = "marius.ionescu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 3, UserId = 3, Email = "admin@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 4, UserId = 4, Email = "organiser@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 5, UserId = 5, Email = "alex.georgescu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 6, UserId = 6, Email = "razvan.moldovan@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 7, UserId = 7, Email = "cristian.stancu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 8, UserId = 8, Email = "andrei.vasilescu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 9, UserId = 9, Email = "florin.dumitru@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 10, UserId = 10, Email = "gabriel.ciobanu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 11, UserId = 11, Email = "lucian.matei@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 12, UserId = 12, Email = "daniel.radu@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 13, UserId = 13, Email = "mihai.popa@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserCredentials { Id = 14, UserId = 14, Email = "stefan.nicolae@gmail.com", Password = "default123", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
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

            modelBuilder.Entity<RatingHistory>().HasData(
                new RatingHistory { Id = 1, UserId = 1, NewRating = 8.5f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 2, UserId = 2, NewRating = 7.8f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 3, UserId = 3, NewRating = 0.0f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 4, UserId = 4, NewRating = 0.0f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 5, UserId = 5, NewRating = 7.2f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 6, UserId = 6, NewRating = 8.1f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 7, UserId = 7, NewRating = 6.9f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 8, UserId = 8, NewRating = 7.7f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 9, UserId = 9, NewRating = 8.3f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 10, UserId = 10, NewRating = 7.4f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 11, UserId = 11, NewRating = 6.8f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 12, UserId = 12, NewRating = 7.9f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 13, UserId = 13, NewRating = 8.0f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RatingHistory { Id = 14, UserId = 14, NewRating = 7.6f, ChangeReason = "Initial Rating", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

    }
}