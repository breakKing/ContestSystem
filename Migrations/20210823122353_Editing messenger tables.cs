using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Editingmessengertables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatsMessages_ChatsMessages_MessageToReplyId",
                table: "ChatsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessages_PrivateMessages_MessageToReplyId",
                table: "PrivateMessages");

            migrationBuilder.DropIndex(
                name: "IX_PrivateMessages_MessageToReplyId",
                table: "PrivateMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatsMessages_MessageToReplyId",
                table: "ChatsMessages");

            migrationBuilder.DropColumn(
                name: "MessageToReplyId",
                table: "PrivateMessages");

            migrationBuilder.DropColumn(
                name: "MessageToReplyId",
                table: "ChatsMessages");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateTimeUTC",
                table: "PrivateMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateTimeUTC",
                table: "ChatsMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateDateTimeUTC",
                table: "PrivateMessages");

            migrationBuilder.DropColumn(
                name: "UpdateDateTimeUTC",
                table: "ChatsMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Chats");

            migrationBuilder.AddColumn<long>(
                name: "MessageToReplyId",
                table: "PrivateMessages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MessageToReplyId",
                table: "ChatsMessages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_MessageToReplyId",
                table: "PrivateMessages",
                column: "MessageToReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsMessages_MessageToReplyId",
                table: "ChatsMessages",
                column: "MessageToReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsMessages_ChatsMessages_MessageToReplyId",
                table: "ChatsMessages",
                column: "MessageToReplyId",
                principalTable: "ChatsMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessages_PrivateMessages_MessageToReplyId",
                table: "PrivateMessages",
                column: "MessageToReplyId",
                principalTable: "PrivateMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
