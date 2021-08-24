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
  <div v-if="can_change_status && currentUserIsOrganizerOfCurrentContest">
    <v-form @submit="saveForm" :validation-schema="schema">
      <div>
        <label class="font-weight-bold">Вердикт</label>
        <v-field as="select" v-model="verdict" class="form-control" name="verdict">
          <option v-for="(value, key) in possible_verdicts" :value="value">{{ key }}</option>
        </v-field>
        <error-message name="verdict"></error-message>
      </div>
      <div>
        <label class="font-weight-bold">Очки</label>
        <v-field type="number" v-model="points" class="form-control" name="points"/>
        <error-message name="points"></error-message>
      </div>
      <button type="submit" class="btn btn-primary">Сохранить</button>
    </v-form>
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
import * as Yup from "yup";
import {ErrorMessage, Field, Form} from "vee-validate";
import CountModes from "../../../../dictionaries/CountModes";

export default {
  name: "SolutionViewComponent",
  mixins: [solution_verdict_readable_presentation, code_editor_mixin],
  components: {
    BreadCrumbsComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  props: ['contest_id', 'solution_id'],
  data() {
    return {
      code: '',
      solution: null,
      problem: null,

      verdict: null,
      points: null,
      schema: Yup.object({
        verdict: Yup.number().required('Укажите вердикт'),
      })
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

      vm.verdict = vm.solution?.actualResult?.verdict || null
      vm.points = vm.solution?.actualResult?.points || null
    })
  },
  methods: {
    ...mapActions(['getSolution', 'changeCurrentContest', 'changeSolution']),
    async saveForm() {
      let solution_data = await this.changeSolution({
        contestId: this.contest_id,
        solutionId: this.solution.id,
        verdict: +this.verdict,
        points: +this.points
      })
      if (solution_data) {
        Object.assign(this.solution, solution_data)
      }
    }
  },
  computed: {
    ...mapGetters([
      'getFormattedFullDateTime',
      'getLastTestNumber',
      'getFormattedMemory',
      'getFormattedTime',
      'currentUser',
      'currentContest',
      'currentUserIsOrganizerOfCurrentContest',
    ]),
    can_change_status() {
      return ![
        TestResultVerdicts.TestInProgress,
        TestResultVerdicts.CompilationError,
        TestResultVerdicts.CompilationSucceed,
        TestResultVerdicts.Undefined,
      ].includes(+this.solution?.actualResult?.verdict)
    },
    possible_verdicts() {
      let items = {
        'Полное решение': TestResultVerdicts.Accepted,
        'Неправильный ответ': TestResultVerdicts.WrongAnswer,
      }
      if (+this.currentContest.rules.countMode !== CountModes.CountPenalty) {
        items['Частичное решение'] = TestResultVerdicts.PartialSolution
      }
      return items
    },
    problemName() {
      return this.solution?.problem?.localizedName || ''
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
  },
}
</script>

<style lang="scss" scoped>

</style>