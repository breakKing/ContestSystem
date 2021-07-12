<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <h2>Решение {{ problemName }}</h2>
  <p><i>({{ getFormattedTime(problem?.timeLimitInMilliseconds) }}, {{ getFormattedMemory(problem?.memoryLimitInBytes) }})</i></p>
  <p>{{ getFormattedFullDateTime(solution?.submitTimeUTC) }}</p>
  <p>{{ verdictName }}</p>
  <p v-if="!isSuccess">Тест: {{ lastTestNumber }}</p>
  <div>
    <div>
      <label>Компилятор:</label>
      <span>{{ solution?.compilerName }}</span>
    </div>
    <div>
      <textarea class="form-control code-input">{{solution?.code}}</textarea>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import * as _ from 'lodash'
import TestResultVerdicts from "../../../../dictionaries/TestResultVerdicts";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import MySolutionViewBreads from "../../../../dictionaries/bread_crumbs/contest/MySolutionViewBreads";

export default {
  name: "SolutionViewComponent",
  components: {BreadCrumbsComponent},
  props: ['contest_id', 'solution_id'],
  data() {
    return {
      solution: null,
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({
        force: false,
        contest_id: vm.contest_id
      })
      vm.solution = await vm.getSolution(vm.solution_id)
    })
  },
  methods: {
    ...mapActions(['getSolution', 'changeCurrentContest', 'getVerdictName', 'getLastTestNumber']),
    getProblemName(problem) {
      return _.find((problem?.localizers || []), (l) => l.culture === 'ru')?.name
    },
    isSuccess(verdict) {
      return +verdict === +TestResultVerdicts.Accepted
    }
  },
  computed: {
    ...mapGetters(['getFormattedFullDateTime', , 'getFormattedMemory', 'getFormattedTime']),
    problemName() {
      return _.find((this.solution?.problem?.localizers || []), (l) => l.culture === 'ru')?.name
    },
    isSuccess() {
      return +this.solution?.verdict === +TestResultVerdicts.Accepted
    },
    verdictName() {
      return this.getVerdictName(this.solution?.verdict)
    },
    lastTestNumber() {
      return this.getLastTestNumber(this.solution)
    },
    bread_crumb_routes() {
      return MySolutionViewBreads(this.contest_id, this.solution_id)
    }
  },
  mounted() {
  },
}
</script>

<style lang="scss" scoped>

</style>