using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Renamingglobalandblogsmoderators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_AspNetUsers_ApprovingGlobalModeratorId",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_ApprovingGlobalModeratorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ApprovingBlogModeratorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Problems_AspNetUsers_ApprovingGlobalModeratorId",
                table: "Problems");

            migrationBuilder.RenameColumn(
                name: "ApprovingGlobalModeratorId",
                table: "Problems",
                newName: "ApprovingModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_ApprovingGlobalModeratorId",
                table: "Problems",
                newName: "IX_Problems_ApprovingModeratorId");

            migrationBuilder.RenameColumn(
                name: "ApprovingBlogModeratorId",
                table: "Posts",
                newName: "ApprovingModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ApprovingBlogModeratorId",
                table: "Posts",
                newName: "IX_Posts_ApprovingModeratorId");

            migrationBuilder.RenameColumn(
                name: "ApprovingGlobalModeratorId",
                table: "Courses",
                newName: "ApprovingModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_ApprovingGlobalModeratorId",
                table: "Courses",
                newName: "IX_Courses_ApprovingModeratorId");

            migrationBuilder.RenameColumn(
                name: "ApprovingGlobalModeratorId",
                table: "Contests",
                newName: "ApprovingModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Contests_ApprovingGlobalModeratorId",
                table: "Contests",
                newName: "IX_Contests_ApprovingModeratorId");

            migrationBuilder.AddColumn<long>(
                name: "ApprovingModeratorId",
                table: "Checkers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationMessage",
                table: "Checkers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checkers_ApprovingModeratorId",
                table: "Checkers",
                column: "ApprovingModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkers_AspNetUsers_ApprovingModeratorId",
                table: "Checkers",
                column: "ApprovingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_AspNetUsers_ApprovingModeratorId",
                table: "Contests",
                column: "ApprovingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_ApprovingModeratorId",
                table: "Courses",
                column: "ApprovingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ApprovingModeratorId",
                table: "Posts",
                column: "ApprovingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_AspNetUsers_ApprovingModeratorId",
                table: "Problems",
                column: "ApprovingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkers_AspNetUsers_ApprovingModeratorId",
                table: "Checkers");

            migrationBuilder.DropForeignKey(
                name: "FK_Contests_AspNetUsers_ApprovingModeratorId",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_ApprovingModeratorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ApprovingModeratorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Problems_AspNetUsers_ApprovingModeratorId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Checkers_ApprovingModeratorId",
                table: "Checkers");

            migrationBuilder.DropColumn(
                name: "ApprovingModeratorId",
                table: "Checkers");

            migrationBuilder.DropColumn(
                name: "ModerationMessage",
                table: "Checkers");

            migrationBuilder.RenameColumn(
                name: "ApprovingModeratorId",
                table: "Problems",
                newName: "ApprovingGlobalModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_ApprovingModeratorId",
                table: "Problems",
                newName: "IX_Problems_ApprovingGlobalModeratorId");

            migrationBuilder.RenameColumn(
                name: "ApprovingModeratorId",
                table: "Posts",
                newName: "ApprovingBlogModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ApprovingModeratorId",
                table: "Posts",
                newName: "IX_Posts_ApprovingBlogModeratorId");

            migrationBuilder.RenameColumn(
                name: "ApprovingModeratorId",
                table: "Courses",
                newName: "ApprovingGlobalModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_ApprovingModeratorId",
                table: "Courses",
                newName: "IX_Courses_ApprovingGlobalModeratorId");

            migrationBuilder.RenameColumn(
                name: "ApprovingModeratorId",
                table: "Contests",
                newName: "ApprovingGlobalModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Contests_ApprovingModeratorId",
                table: "Contests",
                newName: "IX_Contests_ApprovingGlobalModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_AspNetUsers_ApprovingGlobalModeratorId",
                table: "Contests",
                column: "ApprovingGlobalModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_ApprovingGlobalModeratorId",
                table: "Courses",
                column: "ApprovingGlobalModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ApprovingBlogModeratorId",
                table: "Posts",
                column: "ApprovingBlogModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_AspNetUsers_ApprovingGlobalModeratorId",
                table: "Problems",
                column: "ApprovingGlobalModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
