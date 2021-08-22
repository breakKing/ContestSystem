using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Addingsessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RefreshToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresInHours = table.Column<int>(type: "int", nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => new { x.UserId, x.RefreshToken })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Sessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RefreshToken",
                table: "Sessions",
                column: "RefreshToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
