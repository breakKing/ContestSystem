namespace ContestSystem.Models.Dictionaries
{
    public enum FormCheckStatus
    {
        Correct = 0,
        NonExistentCompiler = 1,
        NonExistentParticipant = 2,
        NonExistentContest = 3,
        NonExistentProblem = 4,
        NonExistentRulesSet = 5,
        NonExistentChecker = 6,
        NonExistentUser = 7,
        NotExistentChatUser = 8,
        ExistentSolution = 9,
        LimitExceeded = 10,
        Undefined = 11
    }
}
