namespace ContestSystem.DbStructure.Models.Auth
{
    public class RolesContainer
    {
        public const string Admin = "admin"; // админ сайта
        public const string Moderator = "moderator"; // модератор
        public const string User = "user"; // пользователь (может создавать контесты, участовать в них и клепать посты в блог)
    }
}