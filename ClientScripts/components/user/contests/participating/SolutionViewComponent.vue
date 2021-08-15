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
        <prism-editor v-model="code" 
                      :highlight="highlighter" 
                      :tabSize="4" 
                      line-numbers
                      readonly
                      class="code-editor"/>
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
import code_editor_mixin from "../../../mixins/code_editor_mixin";

export default {
  name: "SolutionViewComponent",
  mixins: [solution_verdict_readable_presentation, code_editor_mixin],
  components: {BreadCrumbsComponent},
  props: ['contest_id', 'solution_id'],
  data() {
    return {
      code: '',
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
      vm.code = vm.solution?.code || ""
    })
  },
  methods: {
    ...mapActions(['getSolution', 'changeCurrentContest']),
    getProblemName(problem) {
      return _.find((problem?.localizers || []), (l) => l.culture === 'ru')?.name
    }
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
    },
    solutionCode() {
      return solution?.code || ""
    }
  },
}
</script>

<style lang="scss" scoped>

</style>