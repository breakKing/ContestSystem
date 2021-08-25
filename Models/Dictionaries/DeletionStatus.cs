namespace ContestSystem.Models.Dictionaries
{
    public enum DeletionStatus
    {
        Undefined = 0,
        Success = 1,
        SuccessWithArchiving = 2,
        NotExistentEntity = 3,
        DbSaveError = 4,
        Blocked = 5
    }
}
