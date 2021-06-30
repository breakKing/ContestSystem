using ContestSystemDbStructure.Enums;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogCreationByNonEqualCurrentUserAndCreator(this ILogger logger, string entityName, long userId, long creatorId)
        {
            logger.LogWarning($"Попытка создать сущность \"{entityName}\" пользователем с идентификатором {userId} при указании в качестве автора пользователя с идентификатором {creatorId}");
        }

        public static void LogCreationFailedBecauseOfLimits(this ILogger logger, string entityName, long userId)
        {
            logger.LogInformation($"Попытка пользователя с идентификатором {userId} создать сущность \"{entityName}\" при наличии у пользователя ограничений на одновременное наличие нескольких таких немодерированных сущностей");
        }

        public static void LogCreationSuccessful(this ILogger logger, string entityName, long createdEntityId, long userId)
        {
            logger.LogInformation($"Пользователем с идентификатором {userId} создана сущность \"{entityName}\" с идентификатором {createdEntityId}");
        }

        public static void LogCreationSuccessfulWithAutoAccept(this ILogger logger, string entityName, long createdEntityId, long userId)
        {
            logger.LogInformation($"Пользователем с идентификатором {userId} создана сущность \"{entityName}\" с идентификатором {createdEntityId}, которая автоматически одобрена по причине доверенного статуса автора");
        }

        public static void LogEditingWithNonEqualFormAndRequestId(this ILogger logger, string entityName, long? formEntityId, long requestEntityId, long userId)
        {
            logger.LogWarning($"Попытка от пользователя с идентификатором {userId} редактировать сущность \"{entityName}\" с идентификатором {requestEntityId}, когда в переданной форме указан идентификатор {formEntityId}");
        }

        public static void LogEditingOfNonExistentEntity(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogWarning($"Попытка редактировать несуществующую сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogEditingByNotAppropriateUser(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogWarning($"Попытка редактировать сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}, который не имеет прав на её редактирование");
        }

        public static void LogEditingSuccessful(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogInformation($"Успешно изменена сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogDeletingOfNonExistentEnitiy(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogWarning($"Попытка удалить несуществующую сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogDeletingByNotAppropriateUser(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogWarning($"Попытка удалить сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}, не являющимся модератором или автором");
        }

        public static void LogDeletingSuccessful(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogInformation($"Удалена сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogDeletingByArchieving(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogInformation($"Архивирована сущность \"{entityName}\" с идентификатором {entityId} пользователем с идентификатором {userId}");
        }

        public static void LogModeratingWithNonEqualFormAndRequestId(this ILogger logger, string entityName, long? formEntityId, long requestEntityId, long userId)
        {
            logger.LogWarning($"Попытка от модератора с идентификатором {userId} модерировать сущность \"{entityName}\" с идентификатором {requestEntityId}, когда в переданной форме указан идентификатор {formEntityId}");
        }

        public static void LogModeratingOfNonExistentEntity(this ILogger logger, string entityName, long entityId, long userId)
        {
            logger.LogWarning($"Попытка модерировать несуществующую сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
        }

        public static void LogModeratingSuccessful(this ILogger logger, string entityName, long entityId, long userId, ApproveType approvalStatus)
        {
            switch (approvalStatus)
            {
                case ApproveType.Rejected:
                    logger.LogInformation($"При модерации отклонена сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
                    break;
                case ApproveType.Accepted:
                    logger.LogInformation($"При модерации одобрена сущность \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
                    break;
                default:
                    logger.LogWarning($"При модерации вынесен вердикт \"Ещё не отмодерирована\" для сущности \"{entityName}\" с идентификатором {entityId} модератором с идентификатором {userId}");
                    break;
            }
        }

        public static void LogParallelSaveError(this ILogger logger, string entityName, long entityId)
        {
            logger.LogWarning($"При сохранении сущности \"{entityName}\" с идентификатором {entityId} произошла ошибка, связанная с параллельным редактированием");
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
