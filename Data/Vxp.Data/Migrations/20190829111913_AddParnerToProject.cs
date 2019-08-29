using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class AddParnerToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartnerId",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PartnerId",
                table: "Projects",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_PartnerId",
                table: "Projects",
                column: "PartnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_PartnerId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PartnerId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Projects");
        }
    }
}
