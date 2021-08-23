using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class AddingContestIdforChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContestId",
                table: "Chats",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ContestId",
                table: "Chats",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Contests_ContestId",
                table: "Chats",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Contests_ContestId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ContestId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ContestId",
                table: "Chats");
        }
    }
}
