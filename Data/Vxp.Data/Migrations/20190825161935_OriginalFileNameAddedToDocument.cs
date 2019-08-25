using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class OriginalFileNameAddedToDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "Documents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "Documents");
        }
    }
}
