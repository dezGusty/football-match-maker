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
        public DbSet<User> Users { get; set; }
        public DbSet<PlayerOrganiser> PlayerOrganisers { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<MatchTeams> MatchTeams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


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

            // MatchTeams configuration
            modelBuilder.Entity<MatchTeams>()
                .HasOne(mt => mt.Match)
                .WithMany()
                .HasForeignKey(mt => mt.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchTeams>()
                .HasOne(mt => mt.Team)
                .WithMany()
                .HasForeignKey(mt => mt.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // TeamPlayers configuration
            modelBuilder.Entity<TeamPlayers>()
                .HasOne(tp => tp.MatchTeam)
                .WithMany()
                .HasForeignKey(tp => tp.MatchTeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamPlayers>()
                .HasOne(tp => tp.Player)
                .WithMany()
                .HasForeignKey(tp => tp.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            modelBuilder.Entity<MatchTeams>()
                .HasIndex(mt => new { mt.MatchId, mt.TeamId })
                .IsUnique();

            modelBuilder.Entity<TeamPlayers>()
                .HasIndex(tp => new { tp.MatchTeamId, tp.PlayerId })
                .IsUnique();

            modelBuilder.Entity<Player>()
                .Property(p => p.Rating)
                .HasColumnType("float");



            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasConversion<int>().IsRequired();
            });

            modelBuilder.Entity<ResetPasswordToken>(entity =>
              {
                  entity.HasKey(e => e.Id);

                  entity.HasIndex(e => e.TokenHash).IsUnique();

                  entity.Property(e => e.TokenHash)
                      .IsRequired()
                      .HasMaxLength(255);

                  entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                  entity.Property(e => e.ExpiresAt)
                      .IsRequired();

                  entity.Property(e => e.UsedAt)
                      .IsRequired(false);


                  entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                  entity.HasIndex(e => new { e.TokenHash, e.ExpiresAt, e.UsedAt })
                      .HasDatabaseName("IX_PasswordResetTokens_TokenHash_ExpiresAt_UsedAt");

                  entity.HasIndex(e => e.UserId)
                      .HasDatabaseName("IX_PasswordResetTokens_UserId");

                  entity.HasIndex(e => e.ExpiresAt)
                      .HasDatabaseName("IX_PasswordResetTokens_ExpiresAt");
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
