namespace ContestSystem.Models
{
    public class UserModelForInitialization
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsLimitedInContests { get; set; } = true;
        public bool IsLimitedInCourses { get; set; } = true;
        public bool IsLimitedInPosts { get; set; } = true;
        public bool IsLimitedInProblems { get; set; } = true;
    }
}