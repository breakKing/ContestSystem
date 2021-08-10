namespace ContestSystem.Models.Dictionaries
{
    public enum CreationStatus
    {
        Success = 0,
        SuccessWithAutoAccept = 1,
        LimitExceeded = 2,
        DbSaveError = 3,
        Undefined = 4
    }
}
