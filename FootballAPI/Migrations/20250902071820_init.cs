using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
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

            migrationBuilder.AlterColumn<int>(
                name: "Goals",
                table: "MatchTeams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FriendRequests",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 7 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 8 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 9 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 10 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 11 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 12 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 13 });

            migrationBuilder.DeleteData(
                table: "PlayerOrganisers",
                keyColumns: new[] { "OrganiserId", "PlayerId" },
                keyValues: new object[] { 4, 14 });

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

            migrationBuilder.AlterColumn<int>(
                name: "Goals",
                table: "MatchTeams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

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
