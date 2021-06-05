using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Addingcreatortoproblemwithoutcascadepaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatorId",
                table: "Problems",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Problems_CreatorId",
                table: "Problems",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_AspNetUsers_CreatorId",
                table: "Problems",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_AspNetUsers_CreatorId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_CreatorId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Problems");
        }
    }
}
