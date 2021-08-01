export default {
    CompilationError: 0,
    CompilationSucceed: 1,
    PresentationError: 2,
    RuntimeError: 3,
    WrongAnswer: 4,
    TimeLimitExceeded: 5,
    MemoryLimitExceeded: 6,
    UnexpectedError: 7,
    PartialSolution: 8,
    Accepted: 9,
    TestInProgress: 10,
    CheckerServersUnavailable: 11,
    // Ситуация, когда программа-чекер выдаёт вердикт fail 
    //(как правило, такое бывает, если участник нашёл решение оптимальнее, 
    // чем решение жюри и программа-чекер учитывает такой поворот событий,
    // чтобы вернуть fail)
    TestlibFail: 12,
    Undefined: 13
}