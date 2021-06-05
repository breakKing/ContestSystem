using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class AddingContestIdtosolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContestId",
                table: "Solutions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_ContestId",
                table: "Solutions",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Contests_ContestId",
                table: "Solutions",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Contests_ContestId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_ContestId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "ContestId",
                table: "Solutions");
        }
    }
}
