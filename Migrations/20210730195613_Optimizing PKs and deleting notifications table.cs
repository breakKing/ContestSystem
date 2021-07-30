using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class OptimizingPKsanddeletingnotificationstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestsResults",
                table: "TestsResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Examples",
                table: "Examples");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesProblems",
                table: "CoursesProblems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesParticipants",
                table: "CoursesParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesLocalModerators",
                table: "CoursesLocalModerators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestsProblems",
                table: "ContestsProblems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestsParticipants",
                table: "ContestsParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestsLocalModerators",
                table: "ContestsLocalModerators");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TestsResults");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Examples");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CoursesProblems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CoursesParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CoursesLocalModerators");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContestsProblems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContestsParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContestsLocalModerators");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestsResults",
                table: "TestsResults",
                columns: new[] { "SolutionId", "Number" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                columns: new[] { "ProblemId", "Number" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Examples",
                table: "Examples",
                columns: new[] { "ProblemId", "Number" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesProblems",
                table: "CoursesProblems",
                columns: new[] { "CourseId", "ProblemId" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesParticipants",
                table: "CoursesParticipants",
                columns: new[] { "CourseId", "ParticipantId" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesLocalModerators",
                table: "CoursesLocalModerators",
                columns: new[] { "CourseId", "LocalModeratorId" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestsProblems",
                table: "ContestsProblems",
                columns: new[] { "ContestId", "ProblemId" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestsParticipants",
                table: "ContestsParticipants",
                columns: new[] { "ContestId", "ParticipantId" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestsLocalModerators",
                table: "ContestsLocalModerators",
                columns: new[] { "ContestId", "LocalModeratorId" })
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TestsResults",
                table: "TestsResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Examples",
                table: "Examples");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesProblems",
                table: "CoursesProblems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesParticipants",
                table: "CoursesParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesLocalModerators",
                table: "CoursesLocalModerators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestsProblems",
                table: "ContestsProblems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestsParticipants",
                table: "ContestsParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestsLocalModerators",
                table: "ContestsLocalModerators");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "TestsResults",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Tests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Examples",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "CoursesProblems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "CoursesParticipants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "CoursesLocalModerators",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ContestsProblems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ContestsParticipants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ContestsLocalModerators",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestsResults",
                table: "TestsResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Examples",
                table: "Examples",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesProblems",
                table: "CoursesProblems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesParticipants",
                table: "CoursesParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesLocalModerators",
                table: "CoursesLocalModerators",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestsProblems",
                table: "ContestsProblems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestsParticipants",
                table: "ContestsParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestsLocalModerators",
                table: "ContestsLocalModerators",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenerationDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }
    }
}
