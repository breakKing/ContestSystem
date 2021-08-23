using Microsoft.EntityFrameworkCore.Migrations;

namespace ContestSystem.Migrations
{
    public partial class Changingenums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Апгрейд VerdictType
            migrationBuilder.Sql($"UPDATE [dbo].[Checkers] SET [CompilationVerdict] = [CompilationVerdict] + 1");
            migrationBuilder.Sql($"UPDATE [dbo].[Checkers] SET [CompilationVerdict] = 0 WHERE [CompilationVerdict] = 14");
            migrationBuilder.Sql($"UPDATE [dbo].[Solutions] SET [Verdict] = [Verdict] + 1");
            migrationBuilder.Sql($"UPDATE [dbo].[Solutions] SET [Verdict] = 0 WHERE [Verdict] = 14");
            migrationBuilder.Sql($"UPDATE [dbo].[TestsResults] SET [Verdict] = [Verdict] + 1");
            migrationBuilder.Sql($"UPDATE [dbo].[TestsResults] SET [Verdict] = 0 WHERE [Verdict] = 14");

            // Апгрейд ChatEventType
            migrationBuilder.Sql($"UPDATE [dbo].[ChatsEvents] SET [Type] = 0 WHERE [Type] = 9");

            // Апгрейд ChatType
            migrationBuilder.Sql($"UPDATE [dbo].[Chats] SET [Type] = [Type] + 1");
            migrationBuilder.Sql($"UPDATE [dbo].[Chats] SET [Type] = 0 WHERE [Type] = 11");

            // Апгрейд RulesCountMode
            migrationBuilder.Sql($"UPDATE [dbo].[RulesSets] SET [CountMode] = 0 WHERE [CountMode] = 4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Даунгрейд VerdictType
            migrationBuilder.Sql($"UPDATE [dbo].[Checkers] SET [CompilationVerdict] = [CompilationVerdict] - 1");
            migrationBuilder.Sql($"UPDATE [dbo].[Checkers] SET [CompilationVerdict] = 13 WHERE [CompilationVerdict] = -1");
            migrationBuilder.Sql($"UPDATE [dbo].[Solutions] SET [Verdict] = [Verdict] - 1");
            migrationBuilder.Sql($"UPDATE [dbo].[Solutions] SET [Verdict] = 13 WHERE [Verdict] = -1");
            migrationBuilder.Sql($"UPDATE [dbo].[TestsResults] SET [Verdict] = [Verdict] - 1");
            migrationBuilder.Sql($"UPDATE [dbo].[TestsResults] SET [Verdict] = 13 WHERE [Verdict] = -1");

            // Даунгрейд ChatEventType
            migrationBuilder.Sql($"UPDATE [dbo].[ChatsEvents] SET [Type] = 9 WHERE [Type] = 0");

            // Даунгрейд ChatType
            migrationBuilder.Sql($"UPDATE [dbo].[Chats] SET [Type] = [Type] - 1");
            migrationBuilder.Sql($"UPDATE [dbo].[Chats] SET [Type] = 10 WHERE [Type] = -1");

            // Даунгрейд RulesCountMode
            migrationBuilder.Sql($"UPDATE [dbo].[RulesSets] SET [CountMode] = 4 WHERE [CountMode] = 0");
        }
    }
}
