<template>
  <bread-crumbs-component v-if="!currentUserIsOrganizerOfCurrentContest"
                          :routes="bread_crumb_routes"></bread-crumbs-component>
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
  <div v-if="currentUserIsOrganizerOfCurrentContest">
    <v-form v-if="can_change_status" @submit="saveForm" :validation-schema="schema">
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
    <template v-if="can_delete">
      <!-- Button trigger modal -->
      <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#delete_solution_modal">
        Удалить
      </button>

      <!-- Modal -->
      <div class="modal fade" id="delete_solution_modal" tabindex="-1" aria-labelledby="delete_solution_modal_label"
           aria-hidden="true">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title" id="delete_solution_modal_label">Подтвердите</h5>
              <button type="button" id="delete_solution_modal_close" class="btn-close" data-bs-dismiss="modal"
                      aria-label="Close"></button>
            </div>
            <div class="modal-body">
              Вы уверены что хотите удалить это решение?
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
              <button type="button" @click.prevent="removeSolution" class="btn btn-primary">Подтвердить</button>
            </div>
          </div>
        </div>
      </div>
    </template>
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
    ...mapActions([
      'getSolution',
      'changeCurrentContest',
      'changeSolution',
      'deleteSolution',
    ]),
    async saveForm() {
      let solution_data = await this.changeSolution({
        contestId: this.contest_id,
        solutionId: this.solution.id,
        verdict: +this.verdict,
        points: +this.points
      })
      if (solution_data) {
        Object.assign(this.solution, solution_data)
        await this.$router.push({name: 'ContestPage', params: {contest_id: this.contest_id}})
      }
    },
    closeModal() {
      document.querySelector('#delete_solution_modal_close').click()
    },
    async removeSolution() {
      let result = await this.deleteSolution({
        contestId: this.contest_id,
        solutionId: this.solution.id,
      })
      if (result) {
        this.closeModal()
        await this.$router.push({name: 'ContestPage', params: {contest_id: this.contest_id}})
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
    can_delete() {
      return [
        TestResultVerdicts.CheckerServersUnavailable,
        TestResultVerdicts.TestlibFail,
        TestResultVerdicts.UnexpectedError
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