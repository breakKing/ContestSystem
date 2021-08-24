namespace ContestSystem.Models.Dictionaries
{
    public enum FormCheckStatus
    {
        Undefined = 0,
        Correct = 1,
        NonExistentCompiler = 2,
        NonExistentParticipant = 3,
        NonExistentContest = 4,
        NonExistentProblem = 5,
        NonExistentRulesSet = 6,
        NonExistentChecker = 7,
        NonExistentUser = 8,
        NonExistentChatUser = 9,
        NonExistentSolution = 10,
        ExistentSolution = 11,
        LimitExceeded = 12,
        WrongMoment = 13
    }
}
