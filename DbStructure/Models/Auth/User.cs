using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ContestSystem.DbStructure.Models.Messenger;
using Microsoft.AspNetCore.Identity;

namespace ContestSystem.DbStructure.Models.Auth
{
    public class User : IdentityUser<long>
    {
        [Required] public string FirstName { get; set; }
        [Required] public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Required] [DataType(DataType.Date)] public DateTime DateOfBirth { get; set; }
        [JsonInclude] public string FullName => $"{Surname} {FirstName} {Patronymic}".Trim();
        public bool IsLimitedInContests { get; set; } = true;
        public bool IsLimitedInPosts { get; set; } = true;
        public bool IsLimitedInCourses { get; set; } = true;
        public bool IsLimitedInProblems { get; set; } = true;
        public bool IsLimitedInRulesSets { get; set; } = true;
        public string Culture { get; set; }

        [InverseProperty("Users")] public virtual List<Role> Roles { get; set; }

        public virtual List<Session> Sessions { get; set; }

        public virtual List<ContestParticipant> ContestParticipants { get; set; }
        public virtual List<Contest> ParticipatingContests { get; set; }

        public virtual List<ContestOrganizer> ContestOrganizers { get; set; }
        public virtual List<Contest> OrganizingContests { get; set; }

        public virtual List<CourseParticipant> CourseParticipants { get; set; }
        public virtual List<Course> ParticipatingCourses { get; set; }

        public virtual List<CourseOrganizer> CourseOrganizers { get; set; }
        public virtual List<Course> OrganizingCourses { get; set; }

        public virtual List<ChatUser> ChatUsers { get; set; }
        public virtual List<Chat> Chats { get; set; }

        public virtual List<Chat> AdminingChats { get; set; }

        public virtual List<Contest> CreatedContests { get; set; }
        public virtual List<Contest> ModeratedContests { get; set; }

        public virtual List<Course> CreatedCourses { get; set; }
        public virtual List<Course> ModeratedCourses { get; set; }

        public virtual List<VirtualContest> VirtualContests { get; set; }

        public virtual List<Solution> Solutions { get; set; }

        public object ResponseStructure =>
            new
            {
                id = Id,
                firstName = FirstName,
                surname = Surname,
                patronymic = Patronymic,
                dateOfBirth = DateOfBirth,
                fullName = FullName,
                userName = UserName,
                normalizedUserName = NormalizedUserName,
                email = Email,
                phoneNumber = PhoneNumber,
                
                // чтобы на клиенте после создания/редактирования знать куда перенаправлять
                limits = new Dictionary<string, bool>()
                {
                    ["courses"] = IsLimitedInCourses,
                    ["contests"] = IsLimitedInContests,
                    ["posts"] = IsLimitedInPosts,
                    ["problems"] = IsLimitedInProblems,
                    ["rules"] = IsLimitedInRulesSets
                }
            };
    }
}