using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class DeletingulongPKspt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBetweenTwoUsers = table.Column<bool>(type: "bit", nullable: false),
                    AnyoneCanJoin = table.Column<bool>(type: "bit", nullable: false),
                    IsCreatedBySystem = table.Column<bool>(type: "bit", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentToReplyId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    CommentToReplyId1 = table.Column<long>(type: "bigint", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    SentDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_CommentToReplyId1",
                        column: x => x.CommentToReplyId1,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContestsHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecondsAfterStart = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<long>(type: "bigint", nullable: false),
                    ContestId = table.Column<long>(type: "bigint", nullable: false),
                    ProblemId = table.Column<long>(type: "bigint", nullable: false),
                    Verdict = table.Column<int>(type: "int", nullable: false),
                    AddedResult = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestsHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestsHistories_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestsHistories_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestsHistories_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatsEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChatId1 = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatsEvents_Chats_ChatId1",
                        column: x => x.ChatId1,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatsUsers",
                columns: table => new
                {
                    ChatId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId1 = table.Column<long>(type: "bigint", nullable: true),
                    ConfirmedByChatAdmin = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedByThemselves = table.Column<bool>(type: "bit", nullable: false),
                    MutedChat = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsUsers", x => new { x.ChatId, x.UserId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_ChatsUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsUsers_Chats_ChatId1",
                        column: x => x.ChatId1,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageToReplyId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    MessageToReplyId1 = table.Column<long>(type: "bigint", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    ChatId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChatId1 = table.Column<long>(type: "bigint", nullable: true),
                    SentDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "IX_Chats_AdminId",
                table: "Chats",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_ChatId1",
                table: "ChatsEvents",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsEvents_UserId",
                table: "ChatsEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsers_ChatId",
                table: "ChatsUsers",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsers_ChatId1",
                table: "ChatsUsers",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsers_UserId",
                table: "ChatsUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentToReplyId1",
                table: "Comments",
                column: "CommentToReplyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_SenderId",
                table: "Comments",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestsHistories_ContestId",
                table: "ContestsHistories",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestsHistories_ParticipantId",
                table: "ContestsHistories",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestsHistories_ProblemId",
                table: "ContestsHistories",
                column: "ProblemId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatsEvents");

            migrationBuilder.DropTable(
                name: "ChatsUsers");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ContestsHistories");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
