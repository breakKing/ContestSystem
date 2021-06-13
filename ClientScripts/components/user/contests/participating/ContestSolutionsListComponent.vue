<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <table class="table">
    <thead>
    <tr>
      <th>Задача</th>
      <th>Компилятор</th>
      <th>Время отправки</th>
      <th>Вердикт</th>
      <th>Очки</th>
      <th>Время</th>
      <th>Память</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="solution of sortedSolutions" :class="{current: +solution.id === +solution_id}">
      <td>{{ solution.problem.localizedName }}</td>
      <td>{{ solution.compilerName }}</td>
      <td>{{ getFormattedFullDateTime(solution.submitTimeUTC) }}</td>
      <td>{{ getVerdictName(solution.verdict) }}{{ additionalVerdictInfo(solution) }}</td>
      <td>{{ solution.points }}</td>
      <td>{{ maxSpentTime(solution.testResults) }}</td>
      <td>{{ maxUsedMemory(solution.testResults) }}</td>
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
  props: ['contest_id', 'solution_id'],
  data() {
    return {
      interval: null,
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'currentContestSolutionsForCurrentUser']),
    sortedSolutions() {
      return _.sortBy(this.currentContestSolutionsForCurrentUser, (s) => +s.id === +this.solution_id)
    },
    bread_crumb_routes() {
      return ContestMySolutionsBreads(this.contest_id)
    }
  },
  methods: {
    ...mapActions(['getUserSolutionsInContest', 'getVerdictName', 'getFormattedFullDateTime']),
    ...mapMutations(['setCurrentContestSolutionsForCurrentUser']),
    async updateList() {
      let solutions = await this.getUserSolutionsInContest({
        contest_id: this.contest_id,
        user_id: this.currentUser?.id
      })
      this.setCurrentContestSolutionsForCurrentUser(solutions)
    },
    maxUsedMemory(results) {
      return _.maxBy(results, (r) => r.usedMemoryInBytes)?.usedMemoryInBytes
    },
    maxSpentTime(results) {
      return _.maxBy(results, (r) => r.usedTimeInMillis)?.usedTimeInMillis
    },
    additionalVerdictInfo(solution) {
      if (+solution?.verdict !== +TestResultVerdicts.Accepted) {
        let lastTestNumber = this.getLastTestNumber(solution);
        if (lastTestNumber) {
          return `Тест ${lastTestNumber}`
        }
      }
      return '';
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.getUserSolutionsInContest({
        contest_id: vm.contest_id,
        user_id: vm.currentUser?.id
      })

      if (!vm.interval) {
        vm.interval = setInterval(vm.updateList, 5000)
      }
    })
  },
  mounted() {
    if (!this.interval) {
      this.interval = setInterval(this.updateList, 5000)
    }
  },
  unmounted() {
    if (this.interval) {
      clearInterval(this.interval)
    }
  },
  beforeRouteLeave(to, from, next) {
    if (this.interval) {
      clearInterval(this.interval)
    }
    next()
  }
}
</script>

<style lang="scss" scoped>

</style>