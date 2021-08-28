using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Smallfixesandadditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentToReplyId1",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Contests_ContestId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentToReplyId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentToReplyId1",
                table: "Comments");

            migrationBuilder.AlterColumn<long>(
                name: "ContestId",
                table: "Solutions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "Solutions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CommentToReplyId",
                table: "Comments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_CourseId",
                table: "Solutions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentToReplyId",
                table: "Comments",
                column: "CommentToReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentToReplyId",
                table: "Comments",
                column: "CommentToReplyId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Contests_ContestId",
                table: "Solutions",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Courses_CourseId",
                table: "Solutions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentToReplyId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Contests_ContestId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Courses_CourseId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_CourseId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentToReplyId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Solutions");

            migrationBuilder.AlterColumn<long>(
                name: "ContestId",
                table: "Solutions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommentToReplyId",
                table: "Comments",
                type: "decimal(20,0)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CommentToReplyId1",
                table: "Comments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentToReplyId1",
                table: "Comments",
                column: "CommentToReplyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentToReplyId1",
                table: "Comments",
                column: "CommentToReplyId1",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Contests_ContestId",
                table: "Solutions",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
