using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Players");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Players",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Players",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Matches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Matches",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4249), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(1819) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4681), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4675) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4684), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4682) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4686), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4685) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4689), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4687) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4691), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4690) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4694), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4692) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4696), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4695) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4699), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4697) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4702), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4700) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4705), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4703) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "DeletedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4707), null, new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4706) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Matches");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "IsAvailable", "IsEnabled" },
                values: new object[] { true, true });
        }
    }
}
