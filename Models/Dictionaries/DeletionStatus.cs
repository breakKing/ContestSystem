namespace ContestSystem.Models.Dictionaries
{
    public enum DeletionStatus
    {
        Success = 0,
        SuccessWithArchiving = 1,
        NotExistentEntity = 2,
        ParallelSaveError = 3,
        Undefined = 4
    }
}
