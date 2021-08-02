import TestResultVerdicts from "../../dictionaries/TestResultVerdicts";

export default {
    methods:{
        actualResult(solution) {
            if (!solution || !solution?.actualResult) {
                return null
            }
            return solution.actualResult
        },
        verdictInfo(actualResult) {
            if (!actualResult) {
                return 'Ожидание'
            }
            let verdict
            switch (+actualResult?.verdict) {
                case TestResultVerdicts.Undefined:
                    verdict = 'Компилируется'
                    break
                case TestResultVerdicts.CompilationError:
                    verdict = 'Ошибка компиляции'
                    break
                case TestResultVerdicts.CompilationSucceed:
                    verdict = 'Выполняется...'
                    break
                case TestResultVerdicts.RuntimeError:
                    verdict = 'Ошибка выполнения на тесте ' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.PresentationError:
                    verdict = 'Ошибка представления на тесте ' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.CheckerServersUnavailable:
                    verdict = 'Сервера проверки недоступны' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.TestlibFail:
                    verdict = 'Ошибка механизма проверки' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.UnexpectedError:
                    verdict = 'Непредвиденная ошибка на тесте' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.MemoryLimitExceeded:
                    verdict = 'Превышение по памяти на тесте ' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.TimeLimitExceeded:
                    verdict = 'Превышение по времени на тесте ' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.WrongAnswer:
                    verdict = 'Неправильный ответ на тесте ' + actualResult.lastRunTestNumber
                    break
                case TestResultVerdicts.PartialSolution:
                    verdict = 'Частичное решение'
                    break
                case TestResultVerdicts.Accepted:
                    verdict = 'Полное решение'
                    break
                case TestResultVerdicts.TestInProgress:
                    verdict = 'Выполняется на тесте ' + (+actualResult.lastRunTestNumber + 1 - +actualResult.testsAreDone)
                    break
            }
            return verdict
        }
    }
}