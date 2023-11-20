using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiddingSystem.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredItemOwnerIdForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    FrozenCredit = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartingPrice = table.Column<double>(type: "float", nullable: false),
                    AvailableForAuction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    EndOfAuction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentPrice = table.Column<double>(type: "float", nullable: false),
                    MinimumBidIncrement = table.Column<double>(type: "float", nullable: false),
                    WinningBidId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AuctionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bids_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bids_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Credit", "Email", "FrozenCredit", "PasswordHash", "UserName" },
                values: new object[,]
                {
                    { 1000, 0.0, "a@a.com", 0.0, "$2a$11$93sTYl9lPaQB3CLPekOTm./v9iyg0XkqY.rYMO7DCVw/h7hp1g0Va", "AAAAA" },
                    { 1001, 0.0, "b@b.com", 0.0, "$2a$11$xICS73GNj4x9tcrUuPBRj.vVS9dL7mm1DVeWdWLf6Ngg7iTg0kZPq", "BBBBB" },
                    { 1002, 0.0, "c@c.com", 0.0, "$2a$11$xjNqNfDdH8nzsPfdrNabhOXR6VDimFK/M4K.hT6Qkxfcq8Iat.JoG", "CCCCC" }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "AvailableForAuction", "Name", "StartingPrice", "UserId" },
                values: new object[,]
                {
                    { 1000, true, "Napoleon's Favorite Hat", 1000.0, 1000 },
                    { 1001, true, "McDonald's Forever Free Nuggets", 230.0, 1002 },
                    { 1002, true, "Eiffel Tower Top Light", 56000.0, 1002 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ItemId",
                table: "Auctions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_UserId",
                table: "Bids",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
