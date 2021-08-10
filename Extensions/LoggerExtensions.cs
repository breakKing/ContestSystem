using ContestSystem.Models.Dictionaries;
using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogCreationByNonEqualCurrentUserAndCreator(this ILogger logger, string entityName,
            long userId, long creatorId)
        {
            logger.LogWarning(
                $"Попытка создать сущность \"{entityName}\" пользователем с идентификатором {userId} при указании в качестве автора пользователя с идентификатором {creatorId}");
        }

        public static void LogCreationFailedBecauseOfLimits(this ILogger logger, string entityName, long userId)
        {
            logger.LogInformation(
                $"Попытка пользователя с идентификатором {userId} создать сущность \"{entityName}\" при наличии у пользователя ограничений на одновременное наличие нескольких таких немодерированных сущностей");
        }

        public static void LogCreationSuccessful(this ILogger logger, string entityName, long createdEntityId,
            long userId)
        {
            logger.LogInformation(
                $"Пользователем с идентификатором {userId} создана сущность \"{entityName}\" с идентификатором {createdEntityId}");
        }

        public static void LogCreationSuccessfulWithAutoAccept(this ILogger logger, string entityName,
            long createdEntityId, long userId)
        {
            logger.LogInformation(
                $"Пользователем с идентификатором {userId} создана сущность \"{entityName}\" с идентификатором {createdEntityId}, которая автоматически одобрена по причине доверенного статуса автора");
        }

        public static void LogCreationUndefinedStatus(this ILogger logger, string entityName,
            long? createdEntityId, long userId)
        {
            logger.LogWarning(
                $"При создании сущности \"{entityName}\" пользователем с идентификатором {userId} был получен статус \"Undefined\" и идентификатор {createdEntityId}");
        }

        public static void LogEditingWithNonEqualFormAndRequestId(this ILogger logger, string entityName,
            long? formEntityId, long requestEntityId, long userId)
        {
            logger.LogWarning(
                $"Попытка от пользователя с идентификатором {userId} редактировать сущность \"{entityName}\" с идентификатором {requestEntityId}, когда в переданной форме указан идентификатор {formEntityId}");
        }

        public static void LogEditingOfNonExistentEntity(this ILogger logger, string entityName, long entityId,
            long userId)
        {
            logger.LogWarning(
                $"Попытка редактировать несуществующую сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogEditingByNotAppropriateUser(this ILogger logger, string entityName, long entityId,
            long userId)
        {
            logger.LogWarning(
                $"Попытка редактировать сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}, который не имеет прав на её редактирование");
        }

        public static void LogEditingSuccessful(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogInformation(
                $"Успешно изменена сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogEditingUndefinedStatus(this ILogger logger, string entityName,
            long entityId, long userId)
        {
            logger.LogWarning(
                $"При редактировании сущности \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId} был получен статус \"Undefined\"");
        }

        public static void LogDeletingOfNonExistentEnitiy(this ILogger logger, string entityName, long entityId,
            long userId)
        {
            logger.LogWarning(
                $"Попытка удалить несуществующую сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogDeletingByNotAppropriateUser(this ILogger logger, string entityName, long entityId,
            long userId)
        {
            logger.LogWarning(
                $"Попытка удалить сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}, не являющимся модератором или автором");
        }

        public static void LogDeletingSuccessful(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogInformation(
                $"Удалена сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogDeletingByArchiving(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogInformation(
                $"Архивирована сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogDeletingUndefinedStatus(this ILogger logger, string entityName,
            long entityId, long userId)
        {
            logger.LogWarning(
                $"При удалении сущности \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId} был получен статус \"Undefined\"");
        }

        public static void LogModeratingWithNonEqualFormAndRequestId(this ILogger logger, string entityName,
            long? formEntityId, long requestEntityId, long userId)
        {
            logger.LogWarning(
                $"Попытка от модератора с идентификатором {userId} модерировать сущность \"{entityName}\" с идентификатором {requestEntityId}, когда в переданной форме указан идентификатор {formEntityId}");
        }

        public static void LogModeratingOfNonExistentEntity(this ILogger logger, string entityName, long entityId,
            long userId)
        {
            logger.LogWarning(
                $"Попытка модерировать несуществующую сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
        }

        public static void LogModeratingByWrongUser(this ILogger logger, string entityName, long entityId, long userId, long approvingModeratorId, ApproveType approvalStatus)
        {
            switch (approvalStatus)
            {
                case ApproveType.Rejected:
                    logger.LogInformation($"Попытка модерировать сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}, " +
                        $"когда данная сущность закреплена за модератором с идентификатором {approvingModeratorId} и отклонена им");
                    break;
                case ApproveType.Accepted:
                    logger.LogInformation($"Попытка модерировать сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}, " +
                        $"когда данная сущность закреплена за модератором с идентификатором {approvingModeratorId} и одобрена им");
                    break;
                default:
                    logger.LogWarning($"Попытка модерировать сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}, " +
                        $"когда данная сущность закреплена за модератором с идентификатором {approvingModeratorId}, но при этом имеет статус \"Ещё не отмодерирована\"");
                    break;
            }
        }

        public static void LogModeratingSuccessful(this ILogger logger, string entityName, long entityId, long userId,
            ApproveType approvalStatus)
        {
            switch (approvalStatus)
            {
                case ApproveType.Rejected:
                    logger.LogInformation(
                        $"При модерации отклонена сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
                    break;
                case ApproveType.Accepted:
                    logger.LogInformation(
                        $"При модерации одобрена сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
                    break;
                default:
                    logger.LogWarning(
                        $"При модерации вынесен вердикт \"Ещё не отмодерирована\" для сущности \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
                    break;
            }
        }

        public static void LogModeratingUndefinedStatus(this ILogger logger, string entityName,
            long entityId, long userId)
        {
            logger.LogWarning(
                $"При модерации сущности \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId} был получен статус \"Undefined\"");
        }

        public static void LogDbSaveError(this ILogger logger, string entityName, long entityId, bool deletion = false)
        {
            if (deletion)
            {
                logger.LogWarning(
                   $"При удалении сущности \"{entityName}\" с идентификатором {entityId} произошла ошибка, связанная с нарушением параллелизма");
            }
            else
            {
                logger.LogWarning(
                   $"При сохранении сущности \"{entityName}\" с идентификатором {entityId} произошла ошибка, связанная с нарушением параллелизма");
            }
        }

        public static void LogNonExistentEntityInForm(this ILogger logger, string entityName, string nonExistentEntityName, long userId)
        {
            logger.LogWarning( $"При проверке формы для сущности \"{entityName}\" от пользователя с идентификатором {userId} было обнаружено " +
                   $"использование несуществующей сущности \"{nonExistentEntityName}\"");
        }

        public static void LogExistentEntityInForm(this ILogger logger, string entityName, string nonExistentEntityName, long userId)
        {
            logger.LogWarning($"При проверке формы для сущности \"{entityName}\" от пользователя с идентификатором {userId} было обнаружено " +
                   $"использование уже существующей сущности \"{nonExistentEntityName}\"");
        }

        public static void LogCreationStatus(this ILogger logger, CreationStatus status, string entityName, long? entityId, long userId)
        {
            switch (status)
            {
                case CreationStatus.Success:
                    logger.LogCreationSuccessful(entityName, entityId.GetValueOrDefault(-1), userId);
                    break;
                case CreationStatus.SuccessWithAutoAccept:
                    logger.LogCreationSuccessfulWithAutoAccept(entityName, entityId.GetValueOrDefault(-1), userId);
                    break;
                case CreationStatus.LimitExceeded:
                    logger.LogCreationFailedBecauseOfLimits(entityName, userId);
                    break;
                case CreationStatus.ParallelSaveError:
                    logger.LogDbSaveError(entityName, userId);
                    break;
                default:
                    logger.LogCreationUndefinedStatus(entityName, entityId, userId);
                    break;
            }
        }

        public static void LogEditionStatus(this ILogger logger, EditionStatus status, string entityName, long entityId, long userId)
        {
            switch (status)
            {
                case EditionStatus.Success:
                    logger.LogEditingSuccessful(entityName, entityId, userId);
                    break;
                case EditionStatus.NotExistentEntity:
                    logger.LogEditingOfNonExistentEntity(entityName, entityId, userId);
                    break;
                case EditionStatus.ParallelSaveError:
                    logger.LogDbSaveError(entityName, entityId);
                    break;
                default:
                    logger.LogEditingUndefinedStatus(entityName, entityId, userId);
                    break;
            }
        }

        public static void LogDeletionStatus(this ILogger logger, DeletionStatus status, string entityName, long entityId, long userId)
        {
            switch (status)
            {
                case DeletionStatus.Success:
                    logger.LogDeletingSuccessful(entityName, entityId, userId);
                    break;
                case DeletionStatus.SuccessWithArchiving:
                    logger.LogDeletingByArchiving(entityName, entityId, userId);
                    break;
                case DeletionStatus.NotExistentEntity:
                    logger.LogDeletingOfNonExistentEnitiy(entityName, entityId, userId);
                    break;
                case DeletionStatus.ParallelSaveError:
                    logger.LogDbSaveError(entityName, entityId, true);
                    break;
                default:
                    logger.LogDeletingUndefinedStatus(entityName, entityId, userId);
                    break;
            }
        }

        public static void LogModerationStatus(this ILogger logger, ModerationStatus status, string entityName, long entityId, long userId)
        {
            switch (status)
            {
                case ModerationStatus.Accepted:
                    logger.LogModeratingSuccessful(entityName, entityId, userId, ApproveType.Accepted);
                    break;
                case ModerationStatus.Rejected:
                    logger.LogModeratingSuccessful(entityName, entityId, userId, ApproveType.Rejected);
                    break;
                case ModerationStatus.NotExistentEntity:
                    logger.LogModeratingOfNonExistentEntity(entityName, entityId, userId);
                    break;
                case ModerationStatus.ParallelSaveError:
                    logger.LogDbSaveError(entityName, entityId);
                    break;
                default:
                    logger.LogModeratingUndefinedStatus(entityName, entityId, userId);
                    break;
            }
        }

        public static void LogFormCheckStatus(this ILogger logger, FormCheckStatus status, string entityName, long userId, string entityId = "")
        {
            switch (status)
            {
                case FormCheckStatus.Correct:
                    break;
                case FormCheckStatus.NonExistentCompiler:
                    logger.LogNonExistentEntityInForm(entityName, Constants.CompilerEntityName, userId);
                    break;
                case FormCheckStatus.NonExistentParticipant:
                    logger.LogWarning($"При проверке формы для сущности \"{entityName}\" от пользователя с идентификатором {userId} было обнаружено " +
                   $"использование несуществующего участника соревнования");
                    break;
                case FormCheckStatus.NonExistentContest:
                    logger.LogNonExistentEntityInForm(entityName, Constants.ContestEntityName, userId);
                    break;
                case FormCheckStatus.NonExistentProblem:
                    logger.LogNonExistentEntityInForm(entityName, Constants.ProblemEntityName, userId);
                    break;
                case FormCheckStatus.NonExistentRulesSet:
                    logger.LogNonExistentEntityInForm(entityName, Constants.RulesSetEntityName, userId);
                    break;
                case FormCheckStatus.NonExistentChecker:
                    logger.LogNonExistentEntityInForm(entityName, Constants.CheckerEntityName, userId);
                    break;
                case FormCheckStatus.NonExistentUser:
                    logger.LogNonExistentEntityInForm(entityName, Constants.UserEntityName, userId);
                    break;
                case FormCheckStatus.ExistentSolution:
                    logger.LogExistentEntityInForm(entityName, Constants.CompilerEntityName, userId);
                    break;
                default:
                    break;
            }
        }

        public static void LogFileWritingFailed(this ILogger logger, string filePath)
        {
            logger.LogWarning($"При записи файла \"{filePath}\" произошла ошибка");
        }

        public static void LogFileWritingSuccessful(this ILogger logger, string filePath)
        {
            logger.LogInformation($"Файл \"{filePath}\" успешно сохранён");
        }

        public static void LogFileDeletingFailed(this ILogger logger, string filePath)
        {
            logger.LogWarning($"При удалении файла \"{filePath}\" произошла ошибка");
        }

        public static void LogFileDeletingSuccessful(this ILogger logger, string filePath)
        {
            logger.LogInformation($"Файл \"{filePath}\" успешно удалён");
        }
    }
}