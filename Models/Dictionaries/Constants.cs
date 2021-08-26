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
        public static readonly int RulesSetsLimitForLimitedUsers = 1;
        public static readonly int ContestLockBeforeStartInMinutes = 60;
        public static readonly int MaxPointsSumForAllTests = 100;

        // Некоторые "настройки" мессенджера
        public static readonly int ChatDefaultOffset = 0;
        public static readonly int ChatDefaultCount = 50;

        // Некоторые "настройки" аутентификации
        public static readonly int ShortTermRefreshTokenLifeTimeInHours = 5;
        public static readonly int LongTermRefreshTokenLifeTimeInHours = 720; // 30 дней
        public static readonly int MaxUserSessionsCount = 5;
        public static readonly string RefreshTokenCookieName = "ContestSystemRT";

        // Названия сущностей
        public static readonly string ContestEntityName = "Contest";
        public static readonly string CourseEntityName = "Course";
        public static readonly string PostEntityName = "Post";
        public static readonly string ProblemEntityName = "Problem";
        public static readonly string CheckerEntityName = "Checker";
        public static readonly string RulesSetEntityName = "RulesSet";
        public static readonly string UserEntityName = "User";
        public static readonly string SolutionEntityName = "Solution";
        public static readonly string CompilerEntityName = "Compiler";
        public static readonly string ChatEntityName = "Chat";

        // Названия для разделов "кодов" ошибок
        public static readonly string CommonSectionName = "Common";
        public static readonly string MessengerSectionName = "Messenger";

        // Наименования ключей для "кодов" ошибок
        public static readonly string ValidationFailedErrorName = "ValidationFailed";
        public static readonly string UserIdMismatchErrorName = "UserIdMismatch";
        public static readonly string EntityIdMismatchErrorName = "EntityIdMismatch";
        public static readonly string EntityDoesntExistErrorName = "EntityDoesntExist";
        public static readonly string EntityLocalizerDoesntExistErrorName = "EntityLocalizerDoesntExist";
        public static readonly string CreationLimitExceededErrorName = "EntityCreationLimitExceeded";
        public static readonly string ModerationByWrongModeratorErrorName = "EntityModerationByWrongModerator";
        public static readonly string UserAlreadyInContestErrorName = "UserAlreadyInContest";
        public static readonly string UserAlreadyInvitedErrorName = "UserAlreadyInvited";
        public static readonly string UserNotInContestErrorName = "UserNotInContest";
        public static readonly string UserNotInChatErrorName = "UserNotInChat";
        public static readonly string UserInsufficientRightsErrorName = "UserInsufficientRights";
        public static readonly string EntityAlreadyExistsErrorName = "EntityAlreadyExists";
        public static readonly string AuthFailedErrorName = "AuthFailed";
        public static readonly string TokenGenerationFailedErrorName = "TokenGenerationFailed";
        public static readonly string UserRegisterFailedErrorName = "UserRegisterFailed";
        public static readonly string VerifyTokenFailedErrorName = "VerifyTokenFailed";
        public static readonly string ChatDoenstExistErrorName = "ChatDoenstExist";
        public static readonly string UserKickedFromChatErrorName = "UserKickedFromChat";
        public static readonly string WrongMomentForEditingErrorName = "WrongMomentForEditing";
        public static readonly string DeleteionBlockedErrorName = "DeleteionBlocked";
        public static readonly string LockedErrorName = "Locked";
        public static readonly string DbSaveErrorName = "DbSaveError";
        public static readonly string CheckerServersUnavailableErrorName = "CheckerServersUnavailable";
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
                { ModerationByWrongModeratorErrorName, "ERR_CONTEST_MODERATION_BY_WRONG_MODERATOR" },
                { LockedErrorName, "ERR_CONTEST_LOCKED" }
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
                { ModerationByWrongModeratorErrorName, "ERR_RULES_MODERATION_BY_WRONG_MODERATOR" }
            },
            [UserEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_USER_VALIDATION_FAILED" },
                { EntityDoesntExistErrorName, "ERR_USER_DOESNT_EXIST" },
                { UserAlreadyInContestErrorName, "ERR_USER_ALREADY_IN_CONTEST" },
                { UserNotInContestErrorName, "ERR_USER_NOT_IN_CONTEST" },
                { UserInsufficientRightsErrorName, "ERR_USER_HAS_INSUFFICIENT_RIGHTS" },
                { EntityAlreadyExistsErrorName, "ERR_USER_ALREADY_EXISTS" },
                { AuthFailedErrorName, "ERR_USER_AUTH_FAILED" },
                { TokenGenerationFailedErrorName, "ERR_USER_TOKEN_GENERATION_FAILED" },
                { UserRegisterFailedErrorName, "ERR_USER_REGISTER_FAILED" },
                { UserAlreadyInvitedErrorName, "ERR_USER_ALREADY_INVITED" },
                { VerifyTokenFailedErrorName, "ERR_USER_VERIFY_TOKEN_FAILED" }
            },
            [SolutionEntityName] = new Dictionary<string, string>
            {
                { ValidationFailedErrorName, "ERR_SOLUTION_VALIDATION_FAILED" },
                { UserIdMismatchErrorName, "ERR_SOLUTION_USER_ID_MISMATCH" },
                { EntityDoesntExistErrorName, "ERR_SOLUTION_DOESNT_EXIST" },
                { CreationLimitExceededErrorName, "ERR_SOLUTION_CREATION_LIMIT_EXCEEDED" },
                { EntityAlreadyExistsErrorName, "ERR_SOLUTION_ALREADY_EXISTS" },
                { WrongMomentForEditingErrorName, "ERR_SOLUTION_CANT_BE_EDITED_NOW" },
                { DeleteionBlockedErrorName, "ERR_SOLUTION_CANT_BE_DELETED" }
            },
            [CompilerEntityName] = new Dictionary<string, string>
            {
                { EntityDoesntExistErrorName, "ERR_COMPILER_DOESNT_EXIST" }
            },
            [MessengerSectionName] = new Dictionary<string, string>
            {
                { UserNotInChatErrorName, "ERR_USER_NOT_IN_CHAT" },
                { ChatDoenstExistErrorName, "ERR_CHAT_DOESNT_EXIST" },
                { UserKickedFromChatErrorName, "ERR_USER_KICKED_FROM_CHAT" }
            },
            [CommonSectionName] = new Dictionary<string, string>
            {
                { DbSaveErrorName, "ERR_DB_SAVE" },
                { CheckerServersUnavailableErrorName, "ERR_CHECKER_SERVERS_UNAVAILABLE" },
                { UndefinedErrorName, "ERR_UNDEFINED" }
            },
        };
    }
}
