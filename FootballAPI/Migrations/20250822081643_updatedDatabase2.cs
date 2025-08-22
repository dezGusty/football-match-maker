using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(7831), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(7833) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9423), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9423) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9426), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9427) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9428), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9428) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9429), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9430) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9431), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9431) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9433), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9433) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9434), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9434) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9436), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9436) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9437), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9438) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9439), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9439) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9440), new DateTime(2025, 8, 22, 8, 16, 43, 30, DateTimeKind.Utc).AddTicks(9441) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4249), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(1819) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4681), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4675) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4684), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4682) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4686), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4685) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4689), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4687) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4691), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4690) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4694), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4692) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4696), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4695) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4699), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4697) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4702), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4700) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4705), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4703) });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4707), new DateTime(2025, 8, 22, 8, 13, 56, 733, DateTimeKind.Utc).AddTicks(4706) });
        }
    }
}
