using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiddingSystem.Migrations
{
    /// <inheritdoc />
    public partial class StartingPriceToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Items");

            migrationBuilder.AddColumn<double>(
                name: "StartingPrice",
                table: "Auctions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSeen = table.Column<bool>(type: "bit", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                columns: new[] { "Credit", "PasswordHash" },
                values: new object[] { 10000.0, "$2a$11$JOw089JRcVZgD2Chk9I3OOA1SBk1R4rVCnRcRSm8FgUuvQJPQjX/m" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "Credit", "PasswordHash" },
                values: new object[] { 10000.0, "$2a$11$lKflb7kK7WYJAVu9DjuAzO6flV3KVttXTmYfA4gPXeWBVTtQZY3I2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1002,
                columns: new[] { "Credit", "PasswordHash" },
                values: new object[] { 10000.0, "$2a$11$.Zss.9FhLsCF59uRVLMKi.E85b/YIjy013SgqYVv3suM5ltSTLMey" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Auctions");

            migrationBuilder.AddColumn<double>(
                name: "StartingPrice",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1000,
                column: "StartingPrice",
                value: 1000.0);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1001,
                column: "StartingPrice",
                value: 230.0);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1002,
                column: "StartingPrice",
                value: 56000.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                columns: new[] { "Credit", "PasswordHash" },
                values: new object[] { 0.0, "$2a$11$93sTYl9lPaQB3CLPekOTm./v9iyg0XkqY.rYMO7DCVw/h7hp1g0Va" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1001,
                columns: new[] { "Credit", "PasswordHash" },
                values: new object[] { 0.0, "$2a$11$xICS73GNj4x9tcrUuPBRj.vVS9dL7mm1DVeWdWLf6Ngg7iTg0kZPq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1002,
                columns: new[] { "Credit", "PasswordHash" },
                values: new object[] { 0.0, "$2a$11$xjNqNfDdH8nzsPfdrNabhOXR6VDimFK/M4K.hT6Qkxfcq8Iat.JoG" });
        }
    }
}
