using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Addingmoremessengerstuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedByChatAdmin",
                table: "ChatsUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedByThemselves",
                table: "ChatsUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MutedChat",
                table: "ChatsUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AnyoneCanJoin",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedBySystem",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ChatsEvents",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChatId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatsEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsEvents_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_ChatId",
                table: "ChatsEvents",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_UserId",
                table: "ChatsEvents",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatsEvents");

            migrationBuilder.DropColumn(
                name: "ConfirmedByChatAdmin",
                table: "ChatsUsers");

            migrationBuilder.DropColumn(
                name: "ConfirmedByThemselves",
                table: "ChatsUsers");

            migrationBuilder.DropColumn(
                name: "MutedChat",
                table: "ChatsUsers");

            migrationBuilder.DropColumn(
                name: "AnyoneCanJoin",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsCreatedBySystem",
                table: "Chats");
        }
    }
}
