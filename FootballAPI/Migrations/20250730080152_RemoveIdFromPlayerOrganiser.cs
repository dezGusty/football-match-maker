using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdFromPlayerOrganiser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerOrganisers",
                table: "PlayerOrganisers");

            migrationBuilder.DropIndex(
                name: "IX_PlayerOrganisers_OrganiserId",
                table: "PlayerOrganisers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PlayerOrganisers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerOrganisers",
                table: "PlayerOrganisers",
                columns: new[] { "OrganiserId", "PlayerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerOrganisers",
                table: "PlayerOrganisers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PlayerOrganisers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerOrganisers",
                table: "PlayerOrganisers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerOrganisers_OrganiserId",
                table: "PlayerOrganisers",
                column: "OrganiserId");
        }
    }
}
