using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class RemoveMessageTopic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_TopicId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TopicId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TopicId",
                table: "Messages",
                column: "TopicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_TopicId",
                table: "Messages",
                column: "TopicId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
