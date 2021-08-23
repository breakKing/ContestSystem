export default {
    Undefined: 0,
    CompilationError: 1,
    CompilationSucceed: 2,
    PresentationError: 3,
    RuntimeError: 4,
    WrongAnswer: 5,
    TimeLimitExceeded: 6,
    MemoryLimitExceeded: 7,
    UnexpectedError: 8,
    PartialSolution: 9,
    Accepted: 10,
    TestInProgress: 11,
    CheckerServersUnavailable: 12,
    // Ситуация, когда программа-чекер выдаёт вердикт fail 
    //(как правило, такое бывает, если участник нашёл решение оптимальнее, 
    // чем решение жюри и программа-чекер учитывает такой поворот событий,
    // чтобы вернуть fail)
    TestlibFail: 13
}