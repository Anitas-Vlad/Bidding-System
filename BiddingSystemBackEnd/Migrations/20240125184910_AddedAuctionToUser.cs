using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiddingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuctionToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Username");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Auctions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                column: "PasswordHash",
                value: "$2a$11$LdtBjpQFPjfcv43wuKKu1Ossj7xlIt8CIc7SCOOwPQd2EMGcHTF6S");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1001,
                column: "PasswordHash",
                value: "$2a$11$gwoVj7C2vGBwPPpQSoKU2OBtvQU/SbTtY9r39HmbDiwbqZtFy/JJ2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1002,
                column: "PasswordHash",
                value: "$2a$11$ApwUYTwoXNIKqCqNgwZI3.LfRxLLF0Ssse9PwxQh9xVbMBSKB7v92");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1003,
                column: "PasswordHash",
                value: "$2a$11$.FRwCx7KzLiGpSyyRVFg7eYqZa.3z7wyCfLf7.fDwzg6GSmqL70KO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1004,
                column: "PasswordHash",
                value: "$2a$11$PjZDhP3OHQ6XhCcIMB8eT.3S8UA9j0WTuvHB3loUKOYKwC1Y.UpNq");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_UserId",
                table: "Auctions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_UserId",
                table: "Auctions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_UserId",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_UserId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "UserName");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                column: "PasswordHash",
                value: "$2a$11$LAdMtfEFNP4O18i/05WF8eCETkIWzt3BV1gDQ6RsF1Po9MmaRpPa.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1001,
                column: "PasswordHash",
                value: "$2a$11$QBw8uksqxPju8DDYw.dor.mt/E8Lx7A7peDvsMoi/HJYZb9Lx8esK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1002,
                column: "PasswordHash",
                value: "$2a$11$ROZaq1OWBXeX0iMuVyXzhOpOJNwmuKRArVfXlSIEeJ/UrEZl1lxNG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1003,
                column: "PasswordHash",
                value: "$2a$11$lX9S.a8ENtBeeeJ18CarIu2Asl4/ugynr8vUhgfrvAXEFCuRtMWze");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1004,
                column: "PasswordHash",
                value: "$2a$11$qWWN0hRYU44aFmvcdge1f.GEX.ftPuUy4/gDNIwCm9gSq0BsUgHSi");
        }
    }
}
