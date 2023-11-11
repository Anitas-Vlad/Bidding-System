using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiddingSystem.Migrations
{
    /// <inheritdoc />
    public partial class LittleChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Items",
                newName: "StartingPrice");

            migrationBuilder.AddColumn<double>(
                name: "CurrentPrice",
                table: "Auctions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                column: "PasswordHash",
                value: "$2a$11$63u.tWLt6w1T6OJk3tk0lefS1gqdWyu4jRpM/RIN3N9uTYhTKUgqO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1001,
                column: "PasswordHash",
                value: "$2a$11$Fj8Td3b0ydTcOrK0tuNRBuLobi21avmq9ebSuEgGqVslZcBylr3eO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1002,
                column: "PasswordHash",
                value: "$2a$11$RAPP1106g8yPC6RGiuKXpujAViMpnw920XSsObiCYQPDXPaw6Vlx2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "StartingPrice",
                table: "Items",
                newName: "Price");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                column: "PasswordHash",
                value: "$2a$11$WfB5W94FIcoruHpgjKp7ue5zEPOgGUYoW7WJ9jh9vMtYTL4osDTxe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1001,
                column: "PasswordHash",
                value: "$2a$11$MtmNAVlXwJh1SmSNKEZhpeKoqzrrJjpATIy/mph.kyTu5SQl1nB.q");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1002,
                column: "PasswordHash",
                value: "$2a$11$/9Ov6plfeMVJRfu53yzvCuMy7F9v/lAP/kCgjGZo/eJYCDCohjiC2");
        }
    }
}
