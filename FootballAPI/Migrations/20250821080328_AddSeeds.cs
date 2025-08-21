using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "Id", "Errors", "FirstName", "IsAvailable", "IsEnabled", "LastName", "ProfileImagePath", "Rating", "Speed", "Stamina", "UserId" },
                values: new object[,]
                {
                    { 1, 2, "Ion", true, true, "Popescu", null, 8.5f, 2, 2, 1 },
                    { 2, 2, "Marius", true, true, "Ionescu", null, 7.8f, 2, 2, 2 },
                    { 3, 2, "Alex", true, true, "Georgescu", null, 7.2f, 2, 2, 5 },
                    { 4, 2, "Razvan", true, true, "Moldovan", null, 8.1f, 2, 2, 6 },
                    { 5, 2, "Cristian", true, true, "Stancu", null, 6.9f, 2, 2, 7 },
                    { 6, 2, "Andrei", true, true, "Vasilescu", null, 7.7f, 2, 2, 8 },
                    { 7, 2, "Florin", true, true, "Dumitru", null, 8.3f, 2, 2, 9 },
                    { 8, 2, "Gabriel", true, true, "Ciobanu", null, 7.4f, 2, 2, 10 },
                    { 9, 2, "Lucian", true, true, "Matei", null, 6.8f, 2, 2, 11 },
                    { 10, 2, "Daniel", true, true, "Radu", null, 7.9f, 2, 2, 12 },
                    { 11, 2, "Mihai", true, true, "Popa", null, 8f, 2, 2, 13 },
                    { 12, 2, "Stefan", true, true, "Nicolae", null, 7.6f, 2, 2, 14 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14);
        }
    }
}
