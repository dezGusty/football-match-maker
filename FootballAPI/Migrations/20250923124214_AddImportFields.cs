using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddImportFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RatingHistories_Users_UserId1",
                table: "RatingHistories");

            migrationBuilder.DropIndex(
                name: "IX_RatingHistories_UserId1",
                table: "RatingHistories");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "RatingHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "RatingHistories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatingHistories_UserId1",
                table: "RatingHistories",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RatingHistories_Users_UserId1",
                table: "RatingHistories",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
