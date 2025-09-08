using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Rating = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Speed = table.Column<int>(type: "INTEGER", nullable: false),
                    Stamina = table.Column<int>(type: "INTEGER", nullable: false),
                    Errors = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()"),
                    ResponsedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Location = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    OrganiserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Users_OrganiserId",
                        column: x => x.OrganiserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganizerDelegates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalOrganizerId = table.Column<int>(type: "INTEGER", nullable: false),
                    DelegateUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReclaimedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizerDelegates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizerDelegates_Users_DelegateUserId",
                        column: x => x.DelegateUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizerDelegates_Users_OriginalOrganizerId",
                        column: x => x.OriginalOrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlayerOrganisers",
                columns: table => new
                {
                    OrganiserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerOrganisers", x => new { x.OrganiserId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_PlayerOrganisers_Users_OrganiserId",
                        column: x => x.OrganiserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlayerOrganisers_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResetPasswordTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResetPasswordTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Goals = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchTeams_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_MatchTeams_MatchTeamId",
                        column: x => x.MatchTeamId,
                        principalTable: "MatchTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "FC Brasov" },
                    { 2, "Steaua Bucuresti" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Email", "Errors", "FirstName", "LastName", "Password", "ProfileImagePath", "Rating", "Role", "Speed", "Stamina", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ion.popescu@gmail.com", 2, "Ion", "Popescu", "default123", null, 8.5f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IonPopescu" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "marius.ionescu@gmail.com", 2, "Marius", "Ionescu", "default123", null, 7.8f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MariusIonescu" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@gmail.com", 2, "Admin", "User", "default123", null, 0f, 0, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "organiser@gmail.com", 2, "Organiser", "User", "default123", null, 0f, 1, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Organiser" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "alex.georgescu@gmail.com", 2, "Alex", "Georgescu", "default123", null, 7.2f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AlexGeorgescu" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "razvan.moldovan@gmail.com", 2, "Razvan", "Moldovan", "default123", null, 8.1f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RazvanMoldovan" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "cristian.stancu@gmail.com", 2, "Cristian", "Stancu", "default123", null, 6.9f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CristianStancu" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "andrei.vasilescu@gmail.com", 2, "Andrei", "Vasilescu", "default123", null, 7.7f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AndreiVasilescu" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "florin.dumitru@gmail.com", 2, "Florin", "Dumitru", "default123", null, 8.3f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FlorinDumitru" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "gabriel.ciobanu@gmail.com", 2, "Gabriel", "Ciobanu", "default123", null, 7.4f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GabrielCiobanu" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "lucian.matei@gmail.com", 2, "Lucian", "Matei", "default123", null, 6.8f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LucianMatei" },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "daniel.radu@gmail.com", 2, "Daniel", "Radu", "default123", null, 7.9f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DanielRadu" },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "mihai.popa@gmail.com", 2, "Mihai", "Popa", "default123", null, 8f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MihaiPopa" },
                    { 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "stefan.nicolae@gmail.com", 2, "Stefan", "Nicolae", "default123", null, 7.6f, 2, 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "StefanNicolae" }
                });

            migrationBuilder.InsertData(
                table: "FriendRequests",
                columns: new[] { "Id", "CreatedAt", "ReceiverId", "ResponsedAt", "SenderId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "PlayerOrganisers",
                columns: new[] { "OrganiserId", "PlayerId", "CreatedAt" },
                values: new object[,]
                {
                    { 4, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_SenderId",
                table: "FriendRequests",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_OrganiserId",
                table: "Matches",
                column: "OrganiserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeams_MatchId",
                table: "MatchTeams",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTeams_TeamId",
                table: "MatchTeams",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizerDelegates_DelegateUserId",
                table: "OrganizerDelegates",
                column: "DelegateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizerDelegates_OriginalOrganizerId_IsActive",
                table: "OrganizerDelegates",
                columns: new[] { "OriginalOrganizerId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerOrganisers_PlayerId",
                table: "PlayerOrganisers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordTokens_UserId",
                table: "ResetPasswordTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_MatchTeamId",
                table: "TeamPlayers",
                column: "MatchTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_UserId",
                table: "TeamPlayers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "OrganizerDelegates");

            migrationBuilder.DropTable(
                name: "PlayerOrganisers");

            migrationBuilder.DropTable(
                name: "ResetPasswordTokens");

            migrationBuilder.DropTable(
                name: "TeamPlayers");

            migrationBuilder.DropTable(
                name: "MatchTeams");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
