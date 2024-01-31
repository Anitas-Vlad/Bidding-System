using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiddingSystem.Migrations
{
    /// <inheritdoc />
    public partial class Refresh : Migration
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
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    EndOfAuction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartingPrice = table.Column<double>(type: "float", nullable: false),
                    CurrentPrice = table.Column<double>(type: "float", nullable: false),
                    MinimumBidIncrement = table.Column<double>(type: "float", nullable: false),
                    WinningBidId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Auctions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
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
                columns: new[] { "Id", "Credit", "Email", "FrozenCredit", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1000, 500.0, "seller1@gmail.com", 0.0, "$2a$11$CYcoJP22/Qr2j3D/QrpNze70NeUlqKp.K2oMoryqXCg2k/WzxO6Q.", "Seller 1" },
                    { 1001, 5000.0, "buyer1@gmail.com", 220.0, "$2a$11$lYLWpgw3Hh3MDc0NZ7UFJ.X./.sGPqPr51Uv78zWe4lYHA6wIcHlW", "Buyer 1" },
                    { 1002, 10000.0, "buyer2@gmail.com", 220.0, "$2a$11$JB.Eu0v/1Itl9WCyDrbeuO900/7PcPHidgQ6IR.F7daAGqtc4eJpq", "Buyer 2" },
                    { 1003, 1500.0, "buyer3@gmail.com", 170.0, "$2a$11$aqceb0ttLKAzayY82HfTB.yTCwH8hVR5v.9lxYGnD8wNIFai.EXmW", "Buyer 3" },
                    { 1004, 0.0, "owner@gmail.com", 0.0, "$2a$11$zBzOUdQOwC1Ly03MD/DlsueOjzUdHvzjPPnLytByVjXRcsNrSQjf2", "Owner" }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "AvailableForAuction", "Name", "UserId" },
                values: new object[,]
                {
                    { 1000, false, "Napoleon's Favorite Hat", 1000 },
                    { 1001, false, "McDonald's Forever Free Nuggets", 1000 },
                    { 1002, false, "Eiffel Tower Top Light", 1000 },
                    { 1003, true, "Test Item 1", 1000 },
                    { 1004, true, "Test Item 2", 1000 },
                    { 1005, true, "Test Item 3", 1000 }
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "CurrentPrice", "EndOfAuction", "ItemId", "MinimumBidIncrement", "SellerId", "StartingPrice", "UserId", "WinningBidId" },
                values: new object[,]
                {
                    { 1000, 100.0, new DateTime(2024, 5, 10, 20, 41, 57, 650, DateTimeKind.Utc).AddTicks(2697), 1000, 10.0, 1000, 100.0, null, 1000 },
                    { 1001, 150.0, new DateTime(2024, 4, 30, 20, 41, 57, 650, DateTimeKind.Utc).AddTicks(2708), 1001, 15.0, 1000, 150.0, null, 1001 },
                    { 1002, 200.0, new DateTime(2024, 5, 10, 20, 41, 57, 650, DateTimeKind.Utc).AddTicks(2710), 1002, 20.0, 1000, 200.0, null, 1002 },
                    { 1003, 200.0, new DateTime(2024, 5, 10, 20, 41, 57, 650, DateTimeKind.Utc).AddTicks(2711), 1003, 20.0, 1000, 200.0, null, 1002 }
                });

            migrationBuilder.InsertData(
                table: "Bids",
                columns: new[] { "Id", "Amount", "AuctionId", "Status", "UserId" },
                values: new object[,]
                {
                    { 1000, 120.0, 1000, 0, 1001 },
                    { 1001, 170.0, 1001, 0, 1003 },
                    { 1002, 220.0, 1002, 0, 1002 },
                    { 1003, 100.0, 1002, 0, 1001 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ItemId",
                table: "Auctions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_UserId",
                table: "Auctions",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
