using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Fixingsessionstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RefreshToken",
                table: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Fingerprint",
                table: "Sessions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                columns: new[] { "UserId", "Fingerprint" })
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Fingerprint",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                columns: new[] { "UserId", "RefreshToken" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RefreshToken",
                table: "Sessions",
                column: "RefreshToken",
                unique: true);
        }
    }
}
