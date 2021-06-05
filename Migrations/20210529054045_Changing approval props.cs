using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Changingapprovalprops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Contests");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Contests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Contests");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Problems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Contests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
