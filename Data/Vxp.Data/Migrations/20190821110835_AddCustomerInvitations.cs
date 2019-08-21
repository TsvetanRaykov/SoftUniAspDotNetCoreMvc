using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vxp.Data.Migrations
{
    public partial class AddCustomerInvitations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    EmailTo = table.Column<string>(nullable: true),
                    DistributorKey = table.Column<string>(nullable: true),
                    TimeSent = table.Column<DateTime>(nullable: false),
                    TimeAccepted = table.Column<DateTime>(nullable: true),
                    SenderId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerInvitations_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvitations_IsDeleted",
                table: "CustomerInvitations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvitations_SenderId",
                table: "CustomerInvitations",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerInvitations");
        }
    }
}
