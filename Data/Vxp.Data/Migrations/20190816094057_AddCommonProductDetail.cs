using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class AddCommonProductDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductDetails");

            migrationBuilder.AddColumn<int>(
                name: "CommonDetailId",
                table: "ProductDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CommonProductDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Measure = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonProductDetails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_CommonDetailId",
                table: "ProductDetails",
                column: "CommonDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonProductDetails_IsDeleted",
                table: "CommonProductDetails",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_CommonProductDetails_CommonDetailId",
                table: "ProductDetails",
                column: "CommonDetailId",
                principalTable: "CommonProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_CommonProductDetails_CommonDetailId",
                table: "ProductDetails");

            migrationBuilder.DropTable(
                name: "CommonProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_CommonDetailId",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "CommonDetailId",
                table: "ProductDetails");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProductDetails",
                nullable: true);
        }
    }
}
