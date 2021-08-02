<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <table class="table">
    <thead>
    <tr>
      <th>Задача</th>
      <th>Компилятор</th>
      <th>Время отправки</th>
      <th>Статус проверки</th>
      <th v-if="pointsAreCounted">Очки</th>
      <th>Время</th>
      <th>Память</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="solution of sortedSolutions"
        @click="$router.push({name: 'SolutionViewPage', params: {solution_id: solution.id}})">
      <td>{{ problemName(solution.problem) }}</td>
      <td>{{ solution.compilerName }}</td>
      <td>{{ getFormattedFullDateTime(solution.submitTimeUTC) }}</td>
      <td>{{ verdictInfo(actualResult(solution)) }}</td>
      <td v-if="pointsAreCounted">{{ (actualResult(solution) && actualResult(solution).points) || 0 }}</td>
      <td>{{ getFormattedTime((actualResult(solution) && actualResult(solution).usedTimeInMillis) || 0) }}</td>
      <td>{{ getFormattedMemory((actualResult(solution) && actualResult(solution).usedMemoryInBytes) || 0) }}</td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from "vuex";
import * as _ from 'lodash'
import CountModes from "../../../../dictionaries/CountModes";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import ContestMySolutionsBreads from "../../../../dictionaries/bread_crumbs/contest/ContestMySolutionsBreads";
import solution_verdict_readable_presentation from "../../../mixins/solution_verdict_readable_presentation";

export default {
  name: "ContestSolutionsListComponent",
  components: {BreadCrumbsComponent},
  mixins: [solution_verdict_readable_presentation],
  props: ['contest_id'],
  computed: {
    ...mapGetters(['currentUser', 'currentContestSolutionsForCurrentUser', 'currentContest']),
    ...mapGetters(['getFormattedFullDateTime', 'getFormattedMemory', 'getFormattedTime']),
    sortedSolutions() {
      return _.sortBy(this.currentContestSolutionsForCurrentUser, (s) => -s.id)
    },
    pointsAreCounted() {
      return +this.currentContest?.rules?.countMode !== +CountModes.CountPenalty
    },
    bread_crumb_routes() {
      return ContestMySolutionsBreads(this.contest_id)
    }
  },
  methods: {
    ...mapActions(['getUserSolutionsInContest', 'changeCurrentContest']),
    ...mapMutations(['setCurrentContestSolutionsForCurrentUser', 'setCurrentContest']),
    problemName(problem) {
      if (!problem) {
        return ''
      }
      let letter = _.find(this.currentContest?.problems || [], (p) => +p.problemId === +problem.id)?.letter + '. ' || ''
      let result
      if (!problem.localizers) {
        result = letter + (problem.localizedName || '')
      } else {
        result = letter + (problem.localizers[0]?.name || '')
      }
      return result
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