<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <table class="table">
    <thead>
    <tr>
      <th>№</th>
      <th>Пользователь</th>
      <th>Решено</th>
      <th>{{ resultsName }}</th>
      <th v-for="letter of tasksLetterList">{{ letter }}</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="entry of currentContestMonitorEntries">
      <td>{{ entry.position }}</td>
      <td>{{ entry.alias }}</td>
      <td>{{ entry.problemsSolvedCount }}</td>
      <td>{{ entry.result }}</td>
      <td v-for="problem_try of sortedTries(entry.problemTries)">
        <div class="d-flex align-content-between justify-content-center">
          <div :class="{green: problem_try.solved}">{{ getFirstRow(problem_try) }}</div>
          <div>{{ getSecondRow(problem_try) }}</div>
          <div v-if="!!getThirdRow(problem_try)">{{ getThirdRow(problem_try) }}</div>
        </div>
      </td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import * as _ from 'lodash'
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import ContestMonitorBreads from "../../../../dictionaries/bread_crumbs/contest/ContestMonitorBreads";

export default {
  name: "ContestMonitoringComponent",
  components: {BreadCrumbsComponent},
  props: ['contest_id'],
  computed: {
    ...mapGetters([
      'currentUser',
      'currentContest',
      'currentContestMonitorEntries',
      'currentUserIsOwnerOfCurrentContest',
      'currentUserIsParticipantOfCurrentContest'
    ]),
    resultsName() {
      if (!this.currentContest) {
        return 'Результат';
      }
      if (+this.currentContest.rules.countMode === 1) {
        return 'Очки'
      }
      if (+this.currentContest.rules.countMode === 2) {
        return 'Штраф'
      }
      if (+this.currentContest.rules.countMode === 3) {
        return 'Счёт'
      }
    },
    tasksLetterList() {
      let first_entry = _.first(this.currentContestMonitorEntries)
      if (!first_entry) {
        return []
      }
      return _.map(this.sortedTries(first_entry.problemTries), (t) => t.letter)
    },
    bread_crumb_routes() {
      return ContestMonitorBreads(this.contest_id)
    }
  },
  methods: {
    ...mapActions(['changeCurrentContest']),
    sortedTries(problemTries) {
      return _.sortBy(problemTries, ['letter'])
    },
    getFirstRow(problem_try) {
      let ret = (problem_try?.solved ? '+' : '-')
      if (+this.currentContest.rules.countMode === 2) {
        ret += (` ${problem_try?.triesCount}`.trimEnd())
      }
      return ret
    },
    getSecondRow(problem_try) {
      return problem_try?.lastTryMinutesAfterStart
    },
    getThirdRow(problem_try) {
      if (+this.currentContest.rules.countMode !== 2) {
        return problem_try.gotPoints
      }
      return ''
    },
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (!(vm.currentUserIsOwnerOfCurrentContest || vm.currentUserIsParticipantOfCurrentContest || vm.currentContest?.rules?.publicMonitor)) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.contest_id}})
      }
    })
  },
}
</script>

<style lang="scss" scoped>
.green {
  color: green;
  font-weight: bold;
}
</style>