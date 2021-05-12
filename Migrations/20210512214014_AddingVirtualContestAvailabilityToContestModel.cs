using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class AddingVirtualContestAvailabilityToContestModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AreVirtualContestsAvailable",
                table: "Contests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreVirtualContestsAvailable",
                table: "Contests");
        }
    }
}
