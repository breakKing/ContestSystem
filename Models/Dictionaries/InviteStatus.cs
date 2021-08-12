namespace ContestSystem.Models.Dictionaries
{
    public enum InviteStatus
    {
        Pending = 0,
        Added = 1,
        UserAlreadyInvited = 2,
        UserAlreadyInEntity = 3,
        DbSaveError = 4,
        Undefined = 5
    }
}
