using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace ContestSystem.Models.Misc
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

        // Названия сущностей для логгера
        public static readonly string ContestEntityName = "Contest";
        public static readonly string CourseEntityName = "Course";
        public static readonly string PostEntityName = "Post";
        public static readonly string ProblemEntityName = "Problem";
        public static readonly string CheckerEntityName = "Checker";
        public static readonly string RulesSetEntityName = "RulesSet";

        // "Коды" ошибок валидаций
        public static readonly string ErrorContestValidationFailed = "ERR_CONTEST_VALIDATION_FAILED";
        public static readonly string ErrorPostValidationFailed = "ERR_POST_VALIDATION_FAILED";
        public static readonly string ErrorCourseValidationFailed = "ERR_COURSE_VALIDATION_FAILED";
        public static readonly string ErrorProblemValidationFailed = "ERR_PROBLEM_VALIDATION_FAILED";
        public static readonly string ErrorCheckerValidationFailed = "ERR_CHECKER_VALIDATION_FAILED";
        public static readonly string ErrorRulesSetValidationFailed = "ERR_RULES_VALIDATION_FAILED";

        // "Коды" ошибок, когда Id текущего пользователя не совпадает с Id пользователя в форме соответствующей сущности
        public static readonly string ErrorContestUserIdMismatch = "ERR_CONTEST_USER_ID_MISMATCH";
        public static readonly string ErrorPostUserIdMismatch = "ERR_POST_USER_ID_MISMATCH";
        public static readonly string ErrorCourseUserIdMismatch = "ERR_COURSE_USER_ID_MISMATCH";
        public static readonly string ErrorProblemUserIdMismatch = "ERR_PROBLEM_USER_ID_MISMATCH";
        public static readonly string ErrorCheckerUserIdMismatch = "ERR_CHECKER_USER_ID_MISMATCH";
        public static readonly string ErrorRulesSetUserIdMismatch = "ERR_RULES_USER_ID_MISMATCH";

        // "Коды" ошибок, когда Id сущности в запросе не совпадает с Id сущности в форме
        public static readonly string ErrorContestIdMismatch = "ERR_CONTEST_ID_MISMATCH";
        public static readonly string ErrorPostIdMismatch = "ERR_POST_ID_MISMATCH";
        public static readonly string ErrorCourseIdMismatch = "ERR_COURSE_ID_MISMATCH";
        public static readonly string ErrorProblemIdMismatch = "ERR_PROBLEM_ID_MISMATCH";
        public static readonly string ErrorCheckerIdMismatch = "ERR_CHECKER_ID_MISMATCH";
        public static readonly string ErrorRulesSetIdMismatch = "ERR_RULES_ID_MISMATCH";

        // "Коды" ошибок, когда сущность пытается редактировать/удалить пользователь, не имеющий к ней отношения
        public static readonly string ErrorContestAffectedByWrongUser = "ERR_CONTEST_AFFECTED_BY_WRONG_USER";
        public static readonly string ErrorPostAffectedByWrongUser = "ERR_POST_AFFECTED_BY_WRONG_USER";
        public static readonly string ErrorCourseAffectedByWrongUser = "ERR_COURSE_AFFECTED_BY_WRONG_USER";
        public static readonly string ErrorProblemAffectedByWrongUser = "ERR_PROBLEM_AFFECTED_BY_WRONG_USER";
        public static readonly string ErrorCheckerAffectedByWrongUser = "ERR_CHECKER_AFFECTED_BY_WRONG_USER";
        public static readonly string ErrorRulesSetAffectedByWrongUser = "ERR_RULES_AFFECTED_BY_WRONG_USER";

        // "Коды" ошибок, когда такая сущность не существует
        public static readonly string ErrorContestDoesntExist = "ERR_CONTEST_DOESNT_EXIST";
        public static readonly string ErrorContestLocalizerDoesntExist = "ERR_CONTEST_LOCALIZER_DOESNT_EXIST";
        public static readonly string ErrorPostDoesntExist = "ERR_POST_DOESNT_EXIST";
        public static readonly string ErrorPostLocalizerDoesntExist = "ERR_POST_LOCALIZER_DOESNT_EXIST";
        public static readonly string ErrorCourseDoesntExist = "ERR_COURSE_DOESNT_EXIST";
        public static readonly string ErrorCourseLocalizerDoesntExist = "ERR_COURSE_LOCALIZER_DOESNT_EXIST";
        public static readonly string ErrorProblemDoesntExist = "ERR_PROBLEM_DOESNT_EXIST";
        public static readonly string ErrorProblemLocalizerDoesntExist = "ERR_PROBLEM_LOCALIZER_DOESNT_EXIST";
        public static readonly string ErrorCheckerDoesntExist = "ERR_CHECKER_DOESNT_EXIST";
        public static readonly string ErrorRulesSetDoesntExist = "ERR_RULES_DOESNT_EXIST";

        // "Коды" ошибок при создании недоверенным пользователем чрезмерного количества сущностей
        public static readonly string ErrorContestCreationLimitExceeded = "ERR_CONTEST_CREATION_LIMIT_EXCEEDED";
        public static readonly string ErrorPostCreationLimitExceeded = "ERR_POST_CREATION_LIMIT_EXCEEDED";
        public static readonly string ErrorCourseCreationLimitExceeded = "ERR_COURSE_CREATION_LIMIT_EXCEEDED";
        public static readonly string ErrorProblemCreationLimitExceeded = "ERR_PROBLEM_CREATION_LIMIT_EXCEEDED";

        // "Код" ошибки из-за сохранения параллельно редактируемых сущностей
        public static readonly string ErrorParallelDbSave = "ERR_PARALLEL_DB_SAVE";
    }
}
