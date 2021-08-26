using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Addingmoderationforrules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "RulesSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ApprovingModeratorId",
                table: "RulesSets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationMessage",
                table: "RulesSets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLimitedInRulesSets",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RulesSets_ApprovingModeratorId",
                table: "RulesSets",
                column: "ApprovingModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_RulesSets_AspNetUsers_ApprovingModeratorId",
                table: "RulesSets",
                column: "ApprovingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("UPDATE [dbo].[AspNetUsers] SET [IsLimitedInRulesSets] = 1 WHERE [UserName] = \'baduser\'");
            migrationBuilder.Sql("UPDATE [dbo].[RulesSets] SET [ApprovalStatus] = 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RulesSets_AspNetUsers_ApprovingModeratorId",
                table: "RulesSets");

            migrationBuilder.DropIndex(
                name: "IX_RulesSets_ApprovingModeratorId",
                table: "RulesSets");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "RulesSets");

            migrationBuilder.DropColumn(
                name: "ApprovingModeratorId",
                table: "RulesSets");

            migrationBuilder.DropColumn(
                name: "ModerationMessage",
                table: "RulesSets");

            migrationBuilder.DropColumn(
                name: "IsLimitedInRulesSets",
                table: "AspNetUsers");
        }
    }
}
