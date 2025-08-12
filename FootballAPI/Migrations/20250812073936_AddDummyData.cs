using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "ImageUrl", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 5, "alex.georgescu@gmail.com", null, "default123", 2, "AlexGeorgescu" },
                    { 6, "razvan.moldovan@gmail.com", null, "default123", 2, "RazvanMoldovan" },
                    { 7, "cristian.stancu@gmail.com", null, "default123", 2, "CristianStancu" },
                    { 8, "andrei.vasilescu@gmail.com", null, "default123", 2, "AndreiVasilescu" },
                    { 9, "florin.dumitru@gmail.com", null, "default123", 2, "FlorinDumitru" },
                    { 10, "gabriel.ciobanu@gmail.com", null, "default123", 2, "GabrielCiobanu" },
                    { 11, "lucian.matei@gmail.com", null, "default123", 2, "LucianMatei" },
                    { 12, "daniel.radu@gmail.com", null, "default123", 2, "DanielRadu" },
                    { 13, "mihai.popa@gmail.com", null, "default123", 2, "MihaiPopa" },
                    { 14, "stefan.nicolae@gmail.com", null, "default123", 2, "StefanNicolae" }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Email", "Errors", "FirstName", "ImageUrl", "IsAvailable", "IsEnabled", "IsPublic", "LastName", "Rating", "Speed", "Stamina" },
                values: new object[,]
                {
                    { 3, "alex.georgescu@gmail.com", 2, "Alex", null, true, true, true, "Georgescu", 7.1999998092651367, 2, 2 },
                    { 4, "razvan.moldovan@gmail.com", 2, "Razvan", null, true, true, true, "Moldovan", 8.1000003814697266, 2, 2 },
                    { 5, "cristian.stancu@gmail.com", 2, "Cristian", null, true, true, true, "Stancu", 6.9000000953674316, 2, 2 },
                    { 6, "andrei.vasilescu@gmail.com", 2, "Andrei", null, true, true, true, "Vasilescu", 7.6999998092651367, 2, 2 },
                    { 7, "florin.dumitru@gmail.com", 2, "Florin", null, true, true, true, "Dumitru", 8.3000001907348633, 2, 2 },
                    { 8, "gabriel.ciobanu@gmail.com", 2, "Gabriel", null, true, true, true, "Ciobanu", 7.4000000953674316, 2, 2 },
                    { 9, "lucian.matei@gmail.com", 2, "Lucian", null, true, true, true, "Matei", 6.8000001907348633, 2, 2 },
                    { 10, "daniel.radu@gmail.com", 2, "Daniel", null, true, true, true, "Radu", 7.9000000953674316, 2, 2 },
                    { 11, "mihai.popa@gmail.com", 2, "Mihai", null, true, true, true, "Popa", 8.0, 2, 2 },
                    { 12, "stefan.nicolae@gmail.com", 2, "Stefan", null, true, true, true, "Nicolae", 7.5999999046325684, 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
