﻿namespace ContestSystem.Models.Dictionaries
{
    public enum EditionStatus
    {
        Success = 0,
        NotExistentEntity = 1,
        DbSaveError = 2,
        ContestLocked = 3,
        ArchivedAndRecreated = 4,
        Undefined = 5
    }
}
