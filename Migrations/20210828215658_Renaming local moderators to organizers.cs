using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Renaminglocalmoderatorstoorganizers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContestsOrganizers",
                columns: table => new
                {
                    ContestId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizerId = table.Column<long>(type: "bigint", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestsOrganizers", x => new { x.ContestId, x.OrganizerId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_ContestsOrganizers_AspNetUsers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestsOrganizers_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoursesOrganizers",
                columns: table => new
                {
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizerId = table.Column<long>(type: "bigint", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesOrganizers", x => new { x.CourseId, x.OrganizerId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_CoursesOrganizers_AspNetUsers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesOrganizers_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestsOrganizers_ContestId",
                table: "ContestsOrganizers",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestsOrganizers_OrganizerId",
                table: "ContestsOrganizers",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesOrganizers_CourseId",
                table: "CoursesOrganizers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesOrganizers_OrganizerId",
                table: "CoursesOrganizers",
                column: "OrganizerId");

            migrationBuilder.Sql("INSERT INTO [dbo].[ContestsOrganizers] ([ContestId], [OrganizerId], [Alias]) SELECT [ContestId], [LocalModeratorId], [Alias] FROM [dbo].[ContestsLocalModerators]");
            migrationBuilder.Sql("INSERT INTO [dbo].[CoursesOrganizers] ([CourseId], [OrganizerId], [Alias]) SELECT [CourseId], [LocalModeratorId], [Alias] FROM [dbo].[CoursesLocalModerators]");

            migrationBuilder.DropTable(
                name: "ContestsLocalModerators");

            migrationBuilder.DropTable(
                name: "CoursesLocalModerators");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContestsLocalModerators",
                columns: table => new
                {
                    ContestId = table.Column<long>(type: "bigint", nullable: false),
                    LocalModeratorId = table.Column<long>(type: "bigint", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestsLocalModerators", x => new { x.ContestId, x.LocalModeratorId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_ContestsLocalModerators_AspNetUsers_LocalModeratorId",
                        column: x => x.LocalModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestsLocalModerators_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoursesLocalModerators",
                columns: table => new
                {
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    LocalModeratorId = table.Column<long>(type: "bigint", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesLocalModerators", x => new { x.CourseId, x.LocalModeratorId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_CoursesLocalModerators_AspNetUsers_LocalModeratorId",
                        column: x => x.LocalModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesLocalModerators_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestsLocalModerators_ContestId",
                table: "ContestsLocalModerators",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestsLocalModerators_LocalModeratorId",
                table: "ContestsLocalModerators",
                column: "LocalModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesLocalModerators_CourseId",
                table: "CoursesLocalModerators",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesLocalModerators_LocalModeratorId",
                table: "CoursesLocalModerators",
                column: "LocalModeratorId");

            migrationBuilder.Sql("INSERT INTO [dbo].[ContestsLocalModerators] ([ContestId], [LocalModeratorId], [Alias]) SELECT [ContestId], [OrganizerId], [Alias] FROM [dbo].[ContestsOrganizers]");
            migrationBuilder.Sql("INSERT INTO [dbo].[CoursesLocalModerators] ([CourseId], [LocalModeratorId], [Alias]) SELECT [CourseId], [OrganizerId], [Alias] FROM [dbo].[CoursesOrganizers]");

            migrationBuilder.DropTable(
                name: "ContestsOrganizers");

            migrationBuilder.DropTable(
                name: "CoursesOrganizers");
        }
    }
}
