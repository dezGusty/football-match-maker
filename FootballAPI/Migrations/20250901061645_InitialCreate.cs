using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DelegatedToUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DelegatedToUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DelegatedToUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDelegatingOrganizer",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DelegatedToUserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelegatingOrganizer",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DelegatedToUserId", "IsDelegatingOrganizer" },
                values: new object[] { null, false });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DelegatedToUserId",
                table: "Users",
                column: "DelegatedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DelegatedToUserId",
                table: "Users",
                column: "DelegatedToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
