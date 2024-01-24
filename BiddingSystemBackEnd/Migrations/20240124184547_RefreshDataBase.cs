using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiddingSystem.Migrations
{
    /// <inheritdoc />
    public partial class RefreshDataBase : Migration
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
                    { 1000, 10000.0, "seller1@gmail.com", 0.0, "$2a$11$LAdMtfEFNP4O18i/05WF8eCETkIWzt3BV1gDQ6RsF1Po9MmaRpPa.", "Seller 1" },
                    { 1001, 10000.0, "buyer1@gmail.com", 0.0, "$2a$11$QBw8uksqxPju8DDYw.dor.mt/E8Lx7A7peDvsMoi/HJYZb9Lx8esK", "Buyer 1" },
                    { 1002, 10000.0, "buyer2@gmail.com", 0.0, "$2a$11$ROZaq1OWBXeX0iMuVyXzhOpOJNwmuKRArVfXlSIEeJ/UrEZl1lxNG", "Buyer 2" },
                    { 1003, 0.0, "buyer3@gmail.com", 0.0, "$2a$11$lX9S.a8ENtBeeeJ18CarIu2Asl4/ugynr8vUhgfrvAXEFCuRtMWze", "Buyer 3" },
                    { 1004, 0.0, "owner@gmail.com", 0.0, "$2a$11$qWWN0hRYU44aFmvcdge1f.GEX.ftPuUy4/gDNIwCm9gSq0BsUgHSi", "Owner" }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "AvailableForAuction", "Name", "UserId" },
                values: new object[,]
                {
                    { 1000, true, "Napoleon's Favorite Hat", 1000 },
                    { 1001, true, "McDonald's Forever Free Nuggets", 1000 },
                    { 1002, true, "Eiffel Tower Top Light", 1002 }
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
