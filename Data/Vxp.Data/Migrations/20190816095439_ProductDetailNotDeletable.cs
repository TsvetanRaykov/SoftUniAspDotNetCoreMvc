using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class ProductDetailNotDeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_IsDeleted",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ProductDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_IsDeleted",
                table: "ProductDetails",
                column: "IsDeleted");
        }
    }
}
