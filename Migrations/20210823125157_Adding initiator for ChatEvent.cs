using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class AddinginitiatorforChatEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatsEvents_AspNetUsers_UserId",
                table: "ChatsEvents");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ChatsEvents",
                newName: "InitiatorId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatsEvents_UserId",
                table: "ChatsEvents",
                newName: "IX_ChatsEvents_InitiatorId");

            migrationBuilder.AddColumn<long>(
                name: "AffectedUserId",
                table: "ChatsEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_AffectedUserId",
                table: "ChatsEvents",
                column: "AffectedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsEvents_AspNetUsers_AffectedUserId",
                table: "ChatsEvents",
                column: "AffectedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsEvents_AspNetUsers_InitiatorId",
                table: "ChatsEvents",
                column: "InitiatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatsEvents_AspNetUsers_AffectedUserId",
                table: "ChatsEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatsEvents_AspNetUsers_InitiatorId",
                table: "ChatsEvents");

            migrationBuilder.DropIndex(
                name: "IX_ChatsEvents_AffectedUserId",
                table: "ChatsEvents");

            migrationBuilder.DropColumn(
                name: "AffectedUserId",
                table: "ChatsEvents");

            migrationBuilder.RenameColumn(
                name: "InitiatorId",
                table: "ChatsEvents",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatsEvents_InitiatorId",
                table: "ChatsEvents",
                newName: "IX_ChatsEvents_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsEvents_AspNetUsers_UserId",
                table: "ChatsEvents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
