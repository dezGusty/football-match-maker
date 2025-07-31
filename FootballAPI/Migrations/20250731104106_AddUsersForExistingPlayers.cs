using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersForExistingPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Players",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "ion.popescu@gmail.com");

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "marius.ionescu@gmail.com");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "ImageUrl", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "ion.popescu@gmail.com", null, "default123", 2, "IonPopescu" },
                    { 2, "marius.ionescu@gmail.com", null, "default123", 2, "MariusIonescu" }
                });

            migrationBuilder.Sql(@"
                UPDATE Players
                SET Email = LOWER(FirstName + '.' + LastName + '@gmail.com')
                WHERE (Email IS NULL OR Email = '')
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Users (Email, Username, Password, Role)
                SELECT
                    Email,
                    FirstName + LastName,
                    'default123',
                    2 -- PLAYER
                FROM Players
                WHERE Email IS NOT NULL
                  AND Email NOT IN (SELECT Email FROM Users)
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Email",
                table: "Players",
                column: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Users_Email",
                table: "Players",
                column: "Email",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Users_Email",
                table: "Players");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Players_Email",
                table: "Players");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Players");
        }
    }
}
