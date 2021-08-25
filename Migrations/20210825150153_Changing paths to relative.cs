using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Changingpathstorelative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Contests] SET [ImagePath] = SUBSTRING([ImagePath], CHARINDEX('Storage', [ImagePath]), LEN([ImagePath]) - CHARINDEX('Storage', [ImagePath]) + 1)");
            migrationBuilder.Sql("UPDATE [dbo].[Posts] SET [ImagePath] = SUBSTRING([ImagePath], CHARINDEX('Storage', [ImagePath]), LEN([ImagePath]) - CHARINDEX('Storage', [ImagePath]) + 1)");
            migrationBuilder.Sql("UPDATE [dbo].[Chats] SET [ImagePath] = SUBSTRING([ImagePath], CHARINDEX('Storage', [ImagePath]), LEN([ImagePath]) - CHARINDEX('Storage', [ImagePath]) + 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
