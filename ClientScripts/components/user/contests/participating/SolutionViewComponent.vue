<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <h2>Решение {{ problemName }}</h2>
  <p><i>({{ problem?.timeLimitInMilliseconds }} мсек, {{ problem?.memoryLimitInBytes }} байт)</i></p>
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
import {mapActions} from "vuex";
import CodeMirror from "codemirror";
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
    ...mapActions(['getSolution', 'changeCurrentContest', 'getVerdictName', 'getLastTestNumber', 'getFormattedFullDateTime']),
    getProblemName(problem) {
      return _.find((problem?.localizers || []), (l) => l.culture === 'ru')?.name
    },
    isSuccess(verdict) {
      return +verdict === +TestResultVerdicts.Accepted
    }
  },
  computed: {
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
    CodeMirror.fromTextArea(document.querySelector('.code-input'), {
      lineNumbers: true,
      readOnly: true,
    })
  },
}
</script>

<style lang="scss" scoped>

</style>