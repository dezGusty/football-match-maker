using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Email", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamAId = table.Column<int>(type: "int", nullable: false),
                    TeamBId = table.Column<int>(type: "int", nullable: false),
                    TeamAGoals = table.Column<int>(type: "int", nullable: false),
                    TeamBGoals = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_TeamAId",
                        column: x => x.TeamAId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_TeamBId",
                        column: x => x.TeamBId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Stamina = table.Column<int>(type: "int", nullable: false),
                    Errors = table.Column<int>(type: "int", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Users_Email",
                        column: x => x.Email,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResetPasswordTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                name: "PlayerMatchHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PerformanceRating = table.Column<double>(type: "float", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMatchHistory_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatchHistory_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatchHistory_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerOrganisers",
                columns: table => new
                {
                    OrganiserId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerOrganisers", x => new { x.OrganiserId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_PlayerOrganisers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerOrganisers_Users_OrganiserId",
                        column: x => x.OrganiserId,
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
                columns: new[] { "Id", "Email", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "ion.popescu@gmail.com", "default123", 2, "IonPopescu" },
                    { 2, "marius.ionescu@gmail.com", "default123", 2, "MariusIonescu" },
                    { 3, "admin@gmail.com", "default123", 0, "Admin" },
                    { 4, "organiser@gmail.com", "default123", 1, "Organiser" },
                    { 5, "alex.georgescu@gmail.com", "default123", 2, "AlexGeorgescu" },
                    { 6, "razvan.moldovan@gmail.com", "default123", 2, "RazvanMoldovan" },
                    { 7, "cristian.stancu@gmail.com", "default123", 2, "CristianStancu" },
                    { 8, "andrei.vasilescu@gmail.com", "default123", 2, "AndreiVasilescu" },
                    { 9, "florin.dumitru@gmail.com", "default123", 2, "FlorinDumitru" },
                    { 10, "gabriel.ciobanu@gmail.com", "default123", 2, "GabrielCiobanu" },
                    { 11, "lucian.matei@gmail.com", "default123", 2, "LucianMatei" },
                    { 12, "daniel.radu@gmail.com", "default123", 2, "DanielRadu" },
                    { 13, "mihai.popa@gmail.com", "default123", 2, "MihaiPopa" },
                    { 14, "stefan.nicolae@gmail.com", "default123", 2, "StefanNicolae" }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Email", "Errors", "FirstName", "IsAvailable", "IsEnabled", "IsPublic", "LastName", "ProfileImagePath", "Rating", "Speed", "Stamina" },
                values: new object[,]
                {
                    { 1, "ion.popescu@gmail.com", 2, "Ion", true, true, true, "Popescu", null, 8.5, 2, 2 },
                    { 2, "marius.ionescu@gmail.com", 2, "Marius", true, true, true, "Ionescu", null, 7.8000001907348633, 2, 2 },
                    { 3, "alex.georgescu@gmail.com", 2, "Alex", true, true, true, "Georgescu", null, 7.1999998092651367, 2, 2 },
                    { 4, "razvan.moldovan@gmail.com", 2, "Razvan", true, true, true, "Moldovan", null, 8.1000003814697266, 2, 2 },
                    { 5, "cristian.stancu@gmail.com", 2, "Cristian", true, true, true, "Stancu", null, 6.9000000953674316, 2, 2 },
                    { 6, "andrei.vasilescu@gmail.com", 2, "Andrei", true, true, true, "Vasilescu", null, 7.6999998092651367, 2, 2 },
                    { 7, "florin.dumitru@gmail.com", 2, "Florin", true, true, true, "Dumitru", null, 8.3000001907348633, 2, 2 },
                    { 8, "gabriel.ciobanu@gmail.com", 2, "Gabriel", true, true, true, "Ciobanu", null, 7.4000000953674316, 2, 2 },
                    { 9, "lucian.matei@gmail.com", 2, "Lucian", true, true, true, "Matei", null, 6.8000001907348633, 2, 2 },
                    { 10, "daniel.radu@gmail.com", 2, "Daniel", true, true, true, "Radu", null, 7.9000000953674316, 2, 2 },
                    { 11, "mihai.popa@gmail.com", 2, "Mihai", true, true, true, "Popa", null, 8.0, 2, 2 },
                    { 12, "stefan.nicolae@gmail.com", 2, "Stefan", true, true, true, "Nicolae", null, 7.5999999046325684, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_MatchDate",
                table: "Matches",
                column: "MatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamAId",
                table: "Matches",
                column: "TeamAId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamBId",
                table: "Matches",
                column: "TeamBId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_MatchId",
                table: "PlayerMatchHistory",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_PlayerId",
                table: "PlayerMatchHistory",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_TeamId",
                table: "PlayerMatchHistory",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerOrganisers_PlayerId",
                table: "PlayerOrganisers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Email",
                table: "Players",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Players_FirstName_LastName",
                table: "Players",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_ExpiresAt",
                table: "ResetPasswordTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_TokenHash_ExpiresAt_UsedAt",
                table: "ResetPasswordTokens",
                columns: new[] { "TokenHash", "ExpiresAt", "UsedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "ResetPasswordTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordTokens_TokenHash",
                table: "ResetPasswordTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name");

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
                name: "PlayerMatchHistory");

            migrationBuilder.DropTable(
                name: "PlayerOrganisers");

            migrationBuilder.DropTable(
                name: "ResetPasswordTokens");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
