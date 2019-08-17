using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class ProductImageNonDeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_ProductImageId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_IsDeleted",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductImages");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductImageId",
                table: "Products",
                column: "ProductImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_ProductImageId",
                table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductImages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductImageId",
                table: "Products",
                column: "ProductImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_IsDeleted",
                table: "ProductImages",
                column: "IsDeleted");
        }
    }
}
