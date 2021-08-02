<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <h2>Решение {{ problemName }}</h2>
  <p><i>({{ getFormattedTime(problem && problem.timeLimitInMilliseconds) }}, {{
      getFormattedMemory(problem && problem.memoryLimitInBytes)
    }})</i></p>
  <p>{{ getFormattedFullDateTime(solution && solution.submitTimeUTC) }}</p>
  <p>{{ verdictName }}</p>
  <p v-if="!isSuccess">Тест: {{ lastTestNumber }}</p>
  <div>
    <div>
      <label>Компилятор:</label>
      <span>{{ solution && solution.compilerName }}</span>
    </div>
    <div>
      <textarea class="form-control code-input">{{solution && solution.code}}</textarea>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import * as _ from 'lodash'
import TestResultVerdicts from "../../../../dictionaries/TestResultVerdicts";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import MySolutionViewBreads from "../../../../dictionaries/bread_crumbs/contest/MySolutionViewBreads";
import solution_verdict_readable_presentation from "../../../mixins/solution_verdict_readable_presentation";

export default {
  name: "SolutionViewComponent",
  mixins: [solution_verdict_readable_presentation],
  components: {BreadCrumbsComponent},
  props: ['contest_id', 'solution_id'],
  data() {
    return {
      solution: null,
      problem: null,
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({
        force: false,
        contest_id: vm.contest_id
      })
      vm.solution = await vm.getSolution(vm.solution_id)
      vm.problem = vm.solution?.problem || null
    })
  },
  methods: {
    ...mapActions(['getSolution', 'changeCurrentContest']),
    getProblemName(problem) {
      return _.find((problem?.localizers || []), (l) => l.culture === 'ru')?.name
    },
  },
  computed: {
    ...mapGetters(['getFormattedFullDateTime', 'getLastTestNumber',
      'getFormattedMemory', 'getFormattedTime']),
    problemName() {
      return _.find((this.solution?.problem?.localizers || []), (l) => l.culture === 'ru')?.name
    },
    isSuccess() {
      return (+this.actualResult(this.solution)?.verdict === +TestResultVerdicts.Accepted) || (+this.actualResult(this.solution)?.verdict === +TestResultVerdicts.PartialSolution)
    },
    verdictName() {
      return this.verdictInfo(this.actualResult(this.solution))
    },
    lastTestNumber() {
      return this.getLastTestNumber(this.solution)
    },
    bread_crumb_routes() {
      return MySolutionViewBreads(this.contest_id, this.solution_id)
    }
  },
}
</script>

<style lang="scss" scoped>

</style>