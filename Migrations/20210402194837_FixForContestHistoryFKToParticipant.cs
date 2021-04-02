using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class FixForContestHistoryFKToParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestsHistories_AspNetUsers_ParticipantId1",
                table: "ContestsHistories");

            migrationBuilder.DropIndex(
                name: "IX_ContestsHistories_ParticipantId1",
                table: "ContestsHistories");

            migrationBuilder.DropColumn(
                name: "ParticipantId1",
                table: "ContestsHistories");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "ContestsHistories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "IX_ContestsHistories_ParticipantId",
                table: "ContestsHistories",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestsHistories_AspNetUsers_ParticipantId",
                table: "ContestsHistories",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestsHistories_AspNetUsers_ParticipantId",
                table: "ContestsHistories");

            migrationBuilder.DropIndex(
                name: "IX_ContestsHistories_ParticipantId",
                table: "ContestsHistories");

            migrationBuilder.AlterColumn<short>(
                name: "ParticipantId",
                table: "ContestsHistories",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId1",
                table: "ContestsHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContestsHistories_ParticipantId1",
                table: "ContestsHistories",
                column: "ParticipantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestsHistories_AspNetUsers_ParticipantId1",
                table: "ContestsHistories",
                column: "ParticipantId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
