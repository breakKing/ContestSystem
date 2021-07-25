using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class StoringcheckerserversinfoinDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CheckerServerId",
                table: "Solutions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CheckerServers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTimeUsedForSolutionUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTimeCompilersUpdatedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckerServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckerServersCompilers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckerServerId = table.Column<long>(type: "bigint", nullable: false),
                    CompilerGUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompilerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckerServersCompilers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckerServersCompilers_CheckerServers_CheckerServerId",
                        column: x => x.CheckerServerId,
                        principalTable: "CheckerServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_CheckerServerId",
                table: "Solutions",
                column: "CheckerServerId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckerServersCompilers_CheckerServerId",
                table: "CheckerServersCompilers",
                column: "CheckerServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_CheckerServers_CheckerServerId",
                table: "Solutions",
                column: "CheckerServerId",
                principalTable: "CheckerServers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_CheckerServers_CheckerServerId",
                table: "Solutions");

            migrationBuilder.DropTable(
                name: "CheckerServersCompilers");

            migrationBuilder.DropTable(
                name: "CheckerServers");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_CheckerServerId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "CheckerServerId",
                table: "Solutions");
        }
    }
}
