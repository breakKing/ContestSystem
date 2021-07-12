<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <table class="table">
    <thead>
    <tr>
      <th>Задача</th>
      <th>Компилятор</th>
      <th>Время отправки</th>
      <th>Статус проверки</th>
      <th>Очки</th>
      <th>Время</th>
      <th>Память</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="solution of sortedSolutions">
      <td>{{ problemName(solution.problem) }}</td>
      <td>{{ solution.compilerName }}</td>
      <td>{{ getFormattedFullDateTime(solution.submitTimeUTC) }}</td>
      <td>{{ verdictInfo(actualResult(solution)) }}</td>
      <td>{{ actualResult(solution)?.points || 0 }}</td>
      <td>{{ getFormattedTime(actualResult(solution)?.usedTimeInMillis || 0) }}</td>
      <td>{{ getFormattedMemory(actualResult(solution)?.usedMemoryInBytes || 0) }}</td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from "vuex";
import * as _ from 'lodash'
import TestResultVerdicts from "../../../../dictionaries/TestResultVerdicts";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import ContestMySolutionsBreads from "../../../../dictionaries/bread_crumbs/contest/ContestMySolutionsBreads";

export default {
  name: "ContestSolutionsListComponent",
  components: {BreadCrumbsComponent},
  props: ['contest_id'],
  computed: {
    ...mapGetters(['currentUser', 'currentContestSolutionsForCurrentUser', 'getVerdictName', 'currentContest']),
    ...mapGetters(['getFormattedFullDateTime', 'getFormattedMemory', 'getFormattedTime']),
    sortedSolutions() {
      return _.sortBy(this.currentContestSolutionsForCurrentUser, (s) => -s.id)
    },
    bread_crumb_routes() {
      return ContestMySolutionsBreads(this.contest_id)
    }
  },
  methods: {
    ...mapActions(['getUserSolutionsInContest', 'changeCurrentContest']),
    ...mapMutations(['setCurrentContestSolutionsForCurrentUser', 'setCurrentContest']),
    actualResult(solution) {
      if (!solution || !solution?.actualResult) {
        return null
      }
      return solution.actualResult
    },
    problemName(problem) {
      if (!problem) {
        return ''
      }
      let letter = _.find(this.currentContest?.problems || [], (p) => +p.problemId === +problem.id)?.letter + '. ' || ''
      let result
      if (!problem.localizers) {
        result = letter + (problem.localizedName || '')
      }
      else {
        result = letter + (problem.localizers[0]?.name || '')
      }
      return result
    },
    verdictInfo(actualResult) {
      if (!actualResult) {
        return 'Ожидание'
      }
      let verdict
      switch (+actualResult.verdict) {
        case TestResultVerdicts.Undefined:
          verdict = 'Компилируется'
          break
        case TestResultVerdicts.CompilationError:
          verdict = 'Ошибка компиляции'
          break
        case TestResultVerdicts.CompilationSucceed:
          verdict = 'Выполняется на тесте 1'
          break
        case TestResultVerdicts.RuntimeError:
          verdict = 'Ошибка выполнения на тесте ' + actualResult.lastRunTestNumber
          break
        case TestResultVerdicts.PresentationError:
          verdict = 'Ошибка представления на тесте ' + actualResult.lastRunTestNumber
          break
        case TestResultVerdicts.ShouldResetSolution:
          verdict = 'Неожиданная ошибка на тесте' + actualResult.lastRunTestNumber
          break
        case TestResultVerdicts.UnexpectedError:
          verdict = 'Неожиданная ошибка на тесте' + actualResult.lastRunTestNumber
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
    },
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (vm.currentContest && vm.currentContestIsInPast) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.currentContest.id}})
      }
      let solutions = await vm.getUserSolutionsInContest({
        contest_id: vm.contest_id,
        user_id: vm.currentUser?.id
      })
      vm.setCurrentContestSolutionsForCurrentUser(solutions)
    })
  }
}
</script>

<style lang="scss" scoped>

</style>