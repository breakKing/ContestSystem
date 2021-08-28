using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Minorrenamingrelatedtoorganizers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestsParticipants_AspNetUsers_ConfirmingLocalModeratorId",
                table: "ContestsParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesParticipants_AspNetUsers_ConfirmingLocalModeratorId",
                table: "CoursesParticipants");

            migrationBuilder.RenameColumn(
                name: "ConfirmingLocalModeratorId",
                table: "CoursesParticipants",
                newName: "ConfirmingOrganizerId");

            migrationBuilder.RenameColumn(
                name: "ConfirmedByLocalModerator",
                table: "CoursesParticipants",
                newName: "ConfirmedByOrganizer");

            migrationBuilder.RenameIndex(
                name: "IX_CoursesParticipants_ConfirmingLocalModeratorId",
                table: "CoursesParticipants",
                newName: "IX_CoursesParticipants_ConfirmingOrganizerId");

            migrationBuilder.RenameColumn(
                name: "ConfirmingLocalModeratorId",
                table: "ContestsParticipants",
                newName: "ConfirmingOrganizerId");

            migrationBuilder.RenameColumn(
                name: "ConfirmedByLocalModerator",
                table: "ContestsParticipants",
                newName: "ConfirmedByOrganizer");

            migrationBuilder.RenameIndex(
                name: "IX_ContestsParticipants_ConfirmingLocalModeratorId",
                table: "ContestsParticipants",
                newName: "IX_ContestsParticipants_ConfirmingOrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestsParticipants_AspNetUsers_ConfirmingOrganizerId",
                table: "ContestsParticipants",
                column: "ConfirmingOrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesParticipants_AspNetUsers_ConfirmingOrganizerId",
                table: "CoursesParticipants",
                column: "ConfirmingOrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestsParticipants_AspNetUsers_ConfirmingOrganizerId",
                table: "ContestsParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesParticipants_AspNetUsers_ConfirmingOrganizerId",
                table: "CoursesParticipants");

            migrationBuilder.RenameColumn(
                name: "ConfirmingOrganizerId",
                table: "CoursesParticipants",
                newName: "ConfirmingLocalModeratorId");

            migrationBuilder.RenameColumn(
                name: "ConfirmedByOrganizer",
                table: "CoursesParticipants",
                newName: "ConfirmedByLocalModerator");

            migrationBuilder.RenameIndex(
                name: "IX_CoursesParticipants_ConfirmingOrganizerId",
                table: "CoursesParticipants",
                newName: "IX_CoursesParticipants_ConfirmingLocalModeratorId");

            migrationBuilder.RenameColumn(
                name: "ConfirmingOrganizerId",
                table: "ContestsParticipants",
                newName: "ConfirmingLocalModeratorId");

            migrationBuilder.RenameColumn(
                name: "ConfirmedByOrganizer",
                table: "ContestsParticipants",
                newName: "ConfirmedByLocalModerator");

            migrationBuilder.RenameIndex(
                name: "IX_ContestsParticipants_ConfirmingOrganizerId",
                table: "ContestsParticipants",
                newName: "IX_ContestsParticipants_ConfirmingLocalModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestsParticipants_AspNetUsers_ConfirmingLocalModeratorId",
                table: "ContestsParticipants",
                column: "ConfirmingLocalModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesParticipants_AspNetUsers_ConfirmingLocalModeratorId",
                table: "CoursesParticipants",
                column: "ConfirmingLocalModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
