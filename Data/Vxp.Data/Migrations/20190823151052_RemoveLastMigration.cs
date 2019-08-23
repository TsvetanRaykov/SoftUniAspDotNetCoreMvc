using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class RemoveLastMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_BuyerId1",
                table: "PriceModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_SellerId1",
                table: "PriceModifiers");

            migrationBuilder.DropIndex(
                name: "IX_PriceModifiers_BuyerId1",
                table: "PriceModifiers");

            migrationBuilder.DropIndex(
                name: "IX_PriceModifiers_SellerId1",
                table: "PriceModifiers");

            migrationBuilder.DropColumn(
                name: "BuyerId1",
                table: "PriceModifiers");

            migrationBuilder.DropColumn(
                name: "SellerId1",
                table: "PriceModifiers");

            migrationBuilder.AlterColumn<string>(
                name: "SellerId",
                table: "PriceModifiers",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "BuyerId",
                table: "PriceModifiers",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_PriceModifiers_BuyerId",
                table: "PriceModifiers",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceModifiers_SellerId",
                table: "PriceModifiers",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_BuyerId",
                table: "PriceModifiers",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_SellerId",
                table: "PriceModifiers",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_BuyerId",
                table: "PriceModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_SellerId",
                table: "PriceModifiers");

            migrationBuilder.DropIndex(
                name: "IX_PriceModifiers_BuyerId",
                table: "PriceModifiers");

            migrationBuilder.DropIndex(
                name: "IX_PriceModifiers_SellerId",
                table: "PriceModifiers");

            migrationBuilder.AlterColumn<int>(
                name: "SellerId",
                table: "PriceModifiers",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "BuyerId",
                table: "PriceModifiers",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "BuyerId1",
                table: "PriceModifiers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SellerId1",
                table: "PriceModifiers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PriceModifiers_BuyerId1",
                table: "PriceModifiers",
                column: "BuyerId1");

            migrationBuilder.CreateIndex(
                name: "IX_PriceModifiers_SellerId1",
                table: "PriceModifiers",
                column: "SellerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_BuyerId1",
                table: "PriceModifiers",
                column: "BuyerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceModifiers_AspNetUsers_SellerId1",
                table: "PriceModifiers",
                column: "SellerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
