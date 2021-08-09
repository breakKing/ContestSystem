using System.Collections.Generic;

namespace ContestSystem.Models.Dictionaries
{
    public static class Constants
    {
        // Некоторые "настройки" системы
        public static readonly int PostsLimitForLimitedUsers = 1;
        public static readonly int ProblemsLimitForLimitedUsers = 1;
        public static readonly int ContestsLimitForLimitedUsers = 1;
        public static readonly int CoursesLimitForLimitedUsers = 1;
        public static readonly int ContestSetupLockBeforeStartInMinutes = 60;
        public static readonly int MaxPointsSumForAllTests = 100;

        // Названия сущностей
        public static readonly string ContestEntityName = "Contest";
        public static readonly string CourseEntityName = "Course";
        public static readonly string PostEntityName = "Post";
        public static readonly string ProblemEntityName = "Problem";
        public static readonly string CheckerEntityName = "Checker";
        public static readonly string RulesSetEntityName = "RulesSet";
        public static readonly string UserEntityName = "User";

        // Названия для разделов "кодов" ошибок
        public static readonly string CommonSectionName = "Common";

        // Наименования ключей для "кодов" ошибок
        public static readonly string ValidationFailedErrorName = "ValidationFailed";
        public static readonly string UserIdMismatchErrorName = "UserIdMismatch";
        public static readonly string EntityIdMismatchErrorName = "EntityIdMismatch";
        public static readonly string EntityDoesntExistErrorName = "EntityDoesntExist";
        public static readonly string EntityLocalizerDoesntExistErrorName = "EntityLocalizerDoesntExist";
        public static readonly string CreationLimitExceededErrorName = "EntityCreationLimitExceeded";
        public static readonly string ModerationByWrongModeratorErrorName = "EntityModerationByWrongModerator";
        public static readonly string UserAlreadyInContestErrorName = "UserAlreadyInContest";
        public static readonly string UserInsufficientRightsErrorName = "UserInsufficientRights";
        public static readonly string UserAlreadyExistsErrorName = "UserAlreadyExists";
        public static readonly string AuthFailedErrorName = "AuthFailed";
        public static readonly string TokenGenerationFailedErrorName = "TokenGenerationFailed";
        public static readonly string UserRegisterFailedErrorName = "UserRegisterFailed";
        public static readonly string ParallelDbSaveErrorName = "ParallelDbSaveError";
        public static readonly string UndefinedErrorName = "UndefinedError";

        // Все "коды" ошибок
        public static readonly Dictionary<string, Dictionary<string, string>> ErrorCodes = new Dictionary<string, Dictionary<string, string>>
        {
            [ContestEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_CONTEST_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_CONTEST_USER_ID_MISMATCH" },
                { EntityIdMismatchErrorName, "ERR_CONTEST_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_CONTEST_DOESNT_EXIST" },
                { EntityLocalizerDoesntExistErrorName, "ERR_CONTEST_LOCALIZER_DOESNT_EXIST" },
                { CreationLimitExceededErrorName, "ERR_CONTEST_CREATION_LIMIT_EXCEEDED" },
                { ModerationByWrongModeratorErrorName, "ERR_CONTEST_MODERATION_BY_WRONG_MODERATOR" }
            },
            [CourseEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_COURSE_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_COURSE_USER_ID_MISMATCH" },
                { EntityIdMismatchErrorName, "ERR_COURSE_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_COURSE_DOESNT_EXIST" },
                { EntityLocalizerDoesntExistErrorName, "ERR_COURSE_LOCALIZER_DOESNT_EXIST" },
                { CreationLimitExceededErrorName, "ERR_COURSE_CREATION_LIMIT_EXCEEDED" },
                { ModerationByWrongModeratorErrorName, "ERR_COURSE_MODERATION_BY_WRONG_MODERATOR" }
            },
            [PostEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_POST_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_POST_USER_ID_MISMATCH" },
                { EntityIdMismatchErrorName, "ERR_POST_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_POST_DOESNT_EXIST" },
                { EntityLocalizerDoesntExistErrorName, "ERR_POST_LOCALIZER_DOESNT_EXIST" },
                { CreationLimitExceededErrorName, "ERR_POST_CREATION_LIMIT_EXCEEDED" },
                { ModerationByWrongModeratorErrorName, "ERR_POST_MODERATION_BY_WRONG_MODERATOR" }
            },
            [ProblemEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_PROBLEM_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_PROBLEM_USER_ID_MISMATCH" },
                { EntityIdMismatchErrorName, "ERR_PROBLEM_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_PROBLEM_DOESNT_EXIST" },
                { EntityLocalizerDoesntExistErrorName, "ERR_PROBLEM_LOCALIZER_DOESNT_EXIST" },
                { CreationLimitExceededErrorName, "ERR_PROBLEM_CREATION_LIMIT_EXCEEDED" },
                { ModerationByWrongModeratorErrorName, "ERR_PROBLEM_MODERATION_BY_WRONG_MODERATOR" }
            },
            [CheckerEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_CHECKER_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_CHECKER_USER_ID_MISMATCH" },
                { EntityIdMismatchErrorName, "ERR_CHECKER_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_CHECKER_DOESNT_EXIST" },
                { ModerationByWrongModeratorErrorName, "ERR_CHECKER_MODERATION_BY_WRONG_MODERATOR" }
            },
            [RulesSetEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_RULES_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_RULES_USER_ID_MISMATCH" },
                { EntityIdMismatchErrorName, "ERR_RULES_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_RULES_DOESNT_EXIST" },
            },
            [UserEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_USER_VALIDATION_FAILED" },
                { EntityDoesntExistErrorName, "ERR_USER_DOESNT_EXIST" },
                { UserAlreadyInContestErrorName, "ERR_USER_ALREADY_IN_CONTEST" },
                { UserInsufficientRightsErrorName, "ERR_USER_HAS_INSUFFICIENT_RIGHTS" },
                { UserAlreadyExistsErrorName, "ERR_USER_ALREADY_EXISTS" },
                { AuthFailedErrorName, "ERR_USER_AUTH_FAILED" },
                { TokenGenerationFailedErrorName, "ERR_USER_TOKEN_GENERATION_FAILED" },
                { UserRegisterFailedErrorName, "ERR_USER_REGISTER_FAILED" }
            },
            [CommonSectionName] = new Dictionary<string, string>
            {
                { ParallelDbSaveErrorName, "ERR_PARALLEL_DB_SAVE" },
                { UndefinedErrorName, "ERR_UNDEFINED" }
            },
        };
    }
}
