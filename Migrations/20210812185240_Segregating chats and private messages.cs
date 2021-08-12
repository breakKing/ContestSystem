using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Segregatingchatsandprivatemessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatsEvents_Chats_ChatId1",
                table: "ChatsEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatsUsers_Chats_ChatId1",
                table: "ChatsUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatsUsers",
                table: "ChatsUsers");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_ChatsUsers_ChatId1",
                table: "ChatsUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatsEvents_ChatId1",
                table: "ChatsEvents");

            migrationBuilder.DropColumn(
                name: "ChatId1",
                table: "ChatsUsers");

            migrationBuilder.DropColumn(
                name: "ChatId1",
                table: "ChatsEvents");

            migrationBuilder.DropColumn(
                name: "IsBetweenTwoUsers",
                table: "Chats");

            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "ChatsUsers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatsUsers",
                table: "ChatsUsers",
                columns: new string[] { "ChatId", "UserId" });

            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "ChatsEvents",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");

            migrationBuilder.CreateTable(
                name: "ChatsMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageToReplyId = table.Column<long>(type: "bigint", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    SentDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatsMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatsMessages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsMessages_ChatsMessages_MessageToReplyId",
                        column: x => x.MessageToReplyId,
                        principalTable: "ChatsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrivateMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageToReplyId = table.Column<long>(type: "bigint", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    SentDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_PrivateMessages_MessageToReplyId",
                        column: x => x.MessageToReplyId,
                        principalTable: "PrivateMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_ChatId",
                table: "ChatsEvents",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsMessages_ChatId",
                table: "ChatsMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsMessages_MessageToReplyId",
                table: "ChatsMessages",
                column: "MessageToReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsMessages_SenderId",
                table: "ChatsMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_MessageToReplyId",
                table: "PrivateMessages",
                column: "MessageToReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_ReceiverId",
                table: "PrivateMessages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_SenderId",
                table: "PrivateMessages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsEvents_Chats_ChatId",
                table: "ChatsEvents",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsUsers_Chats_ChatId",
                table: "ChatsUsers",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatsEvents_Chats_ChatId",
                table: "ChatsEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatsUsers_Chats_ChatId",
                table: "ChatsUsers");

            migrationBuilder.DropTable(
                name: "ChatsMessages");

            migrationBuilder.DropTable(
                name: "PrivateMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatsEvents_ChatId",
                table: "ChatsEvents");

            migrationBuilder.DropPrimaryKey(
               name: "PK_ChatsUsers",
               table: "ChatsUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChatId",
                table: "ChatsUsers",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatsUsers",
                table: "ChatsUsers",
                columns: new string[] { "ChatId", "UserId" });

            migrationBuilder.AddColumn<long>(
                name: "ChatId1",
                table: "ChatsUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ChatId",
                table: "ChatsEvents",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ChatId1",
                table: "ChatsEvents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBetweenTwoUsers",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChatId1 = table.Column<long>(type: "bigint", nullable: true),
                    MessageToReplyId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    MessageToReplyId1 = table.Column<long>(type: "bigint", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    SentDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId1",
                        column: x => x.ChatId1,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_MessageToReplyId1",
                        column: x => x.MessageToReplyId1,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsers_ChatId1",
                table: "ChatsUsers",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_ChatId1",
                table: "ChatsEvents",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId1",
                table: "Messages",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageToReplyId1",
                table: "Messages",
                column: "MessageToReplyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsEvents_Chats_ChatId1",
                table: "ChatsEvents",
                column: "ChatId1",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsUsers_Chats_ChatId1",
                table: "ChatsUsers",
                column: "ChatId1",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
