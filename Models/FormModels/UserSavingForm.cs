using System;
using System.Collections.Generic;

namespace ContestSystem.Models.FormModels
{
    public class UserSavingForm
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<string> Roles { get; set; }
        public bool IsLimitedInContests { get; set; }
        public bool IsLimitedInPosts { get; set; }
        public bool IsLimitedInCourses { get; set; }
        public bool IsLimitedInProblems { get; set; }
    }
}