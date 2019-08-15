using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class CategoryProductNameRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductCategories",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductCategories",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
