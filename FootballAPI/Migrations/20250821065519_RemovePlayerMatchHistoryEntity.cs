using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlayerMatchHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerMatchHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerMatchHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    MatchTeamsId = table.Column<int>(type: "int", nullable: true),
                    PerformanceRating = table.Column<double>(type: "float", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMatchHistory_MatchTeams_MatchTeamsId",
                        column: x => x.MatchTeamsId,
                        principalTable: "MatchTeams",
                        principalColumn: "Id");
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

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_MatchId",
                table: "PlayerMatchHistory",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_MatchTeamsId",
                table: "PlayerMatchHistory",
                column: "MatchTeamsId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_PlayerId",
                table: "PlayerMatchHistory",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchHistory_TeamId",
                table: "PlayerMatchHistory",
                column: "TeamId");
        }
    }
}
