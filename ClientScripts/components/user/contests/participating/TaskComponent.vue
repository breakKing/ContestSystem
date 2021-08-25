<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <div class="row">
    <div class="col-12 col-md-3">
      <contest-side-bar-component
          :mapped_solutions="mappedSolutionsForCurrentContest"
          :active_task_id="actualTaskId"
          :tasks="orderedTasks"
          :chats="getContestChats(currentContest?.id)"
      ></contest-side-bar-component>
    </div>
    <div class="col">
      <router-link class="btn btn-info" :to="{name: 'ContestMySolutionsPage', params:{contest_id: contest_id}}">Мои
        попытки
      </router-link>

      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <h2>{{ problem_name }}</h2>
      <p><i>({{ getFormattedTime(problem?.timeLimitInMilliseconds || 0) }},
        {{ getFormattedMemory(problem?.memoryLimitInBytes || 0) }})</i></p>
      <p v-html="problem_description" v-if="problem_description"></p>
      <p v-html="problem_input" v-if="problem_input"></p>
      <p v-html="problem_output" v-if="problem_output"></p>
      <template v-if="sortedExamples.length > 0">
        <div class="row">
          <div class="col">Вход</div>
          <div class="col">Выход</div>
        </div>
        <div class="row" v-for="example of sortedExamples">
          <!--если в строках есть \n, то они заменяются пробелами, но оборачивание в тег pre это фиксит-->
          <div class="col">
            <pre>{{ example.inputText }}</pre>
          </div>
          <div class="col">
            <pre>{{ example.outputText }}</pre>
          </div>
        </div>
      </template>

      <h2>Решение</h2>
      <v-form @submit="onSubmitSolution" :validation-schema="solutionSchema" class="mb-3">
        <div>
          <v-field v-model="code" name="code" type="hidden"/>
          <prism-editor v-model="code"
                        :highlight="highlighter"
                        :tabSize="4"
                        line-numbers
                        class="code-editor"/>
          <error-message name="code"></error-message>
        </div>
        <div>
          <label>Компилятор</label>
          <v-field :disabled="loading" as="select" v-model="compiler" class="form-control" name="compiler">
            <option :value="cpm.guid" v-for="cpm of availableCompilers">{{ cpm.name }}</option>
          </v-field>
          <error-message name="compiler"></error-message>
        </div>
        <div>
          <p>Попыток осталось: {{ triesLeft }}</p>
        </div>
        <div class="form-group mt-3">
          <button class="btn btn-primary" :disabled="loading" type="submit">Отправить</button>
        </div>
      </v-form>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from "vuex";
import TasksNavigationComponent from "./TasksNavigationComponent";
import * as _ from "lodash";
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import ContestParticipatingTaskBreads
  from "../../../../dictionaries/bread_crumbs/contest/ContestParticipatingTaskBreads";
import TestResultVerdicts from "../../../../dictionaries/TestResultVerdicts";
import code_editor_mixin from '../../../mixins/code_editor_mixin';
import ChatListComponent from "../../../chats/ChatListComponent";
import ContestSideBarComponent from "./ContestSideBarComponent";

export default {
  name: "TaskComponent",
  components: {
    ContestSideBarComponent,
    ChatListComponent,
    BreadCrumbsComponent,
    TasksNavigationComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage
  },
  props: ['contest_id', 'task_id'],
  mixins: [code_editor_mixin],
  data() {
    return {
      loading: false,
      problem: null,
      compiler: null,
      error_msg: '',
      code: '',

      solutionSchema: Yup.object({
        compiler: Yup.string().nullable().required(),
        code: Yup.string().required(),
      })
    }
  },
  computed: {
    ...mapGetters([
      'currentUser',
      'currentContest',
      'currentContestRulesSet',
      'currentContestUserStats',
      'currentContestSolutionsForCurrentUser', // тут инфа о попытках каждого таска(для навигатора)
      'availableCompilers',
      'currentContestIsInPast',
      'getFormattedMemory',
      'getFormattedTime',
      'getContestChats',
      'mappedSolutionsForCurrentContest',
    ]),
    orderedTasks() {
      return _.sortBy((this.currentContest?.problems || []), ['letter'])
    },
    actualTaskId() {
      let id
      if (this.task_id) {
        id = +this.task_id
      } else {
        id = +_.first(this.orderedTasks)?.problemId || null
      }
      this.problem = _.find(this.orderedTasks || [], (t) => +t.problemId === +id)?.problem || null
      return +id
    },
    problem_name() {
      return this.problem?.localizedName
    },
    problem_description() {
      return this.problem?.localizedDescription
    },
    problem_input() {
      return this.problem?.localizedInputBlock
    },
    problem_output() {
      return this.problem?.localizedOutputBlock
    },
    sortedExamples() {
      return _.sortBy((this.problem?.examples || []), ['number'])
    },
    bread_crumb_routes() {
      return ContestParticipatingTaskBreads(this.contest_id, this.task_id)
    },
    triesLeft() {
      let triesCount = _.find(this.currentContestUserStats?.problemTries || [], (pt) => +pt.problemId === +this.actualTaskId)?.triesCount || 0
      return (this.currentContestRulesSet?.maxTriesForOneProblem || 0) - triesCount
    }
  },
  methods: {
    ...mapActions([
      'changeCurrentContest',
      'fetchAvailableCompilers',
      'getSolution',
      'sendSolution',
      'compileSolution',
      'runSolutionTests',
      'getUserSolutionsInContest',
      'updateOrAddSolutionToState',
    ]),
    async onSubmitSolution() {
      this.loading = true
      let {data, status, errors} = await this.sendSolution({
        code: this.code,
        compilerGUID: this.compiler,
        compilerName: _.find(this.availableCompilers || [], (c, i) => c.guid === this.compiler)?.name,
        contestId: this.contest_id,
        userId: this.currentUser.id,
        problemId: this.actualTaskId,
      })
      if (status && data) {
        this.error_msg = ''
        let solutionId = data
        this.compileSolution(solutionId).then(async () => {
          let newSolution = await this.getSolution(solutionId)
          if (newSolution) {
            await this.updateOrAddSolutionToState({
              current_solutions_collection: this.currentContestSolutionsForCurrentUser,
              solution_data: newSolution,
              is_solution_data_full: true,
              update_callback: ({index, props, solution_user_id}) => {
                if (+index > -1 && solution_user_id && +solution_user_id === +this.currentUser.id) {
                  this.updateCurrentContestSolutionForCurrentUser({index, props})
                }
                return props
              }
            })
            if (+newSolution.actualResult?.verdict === TestResultVerdicts.CompilationSucceed) {
              this.runSolutionTests(solutionId)
            }
          }
        })
        await this.$router.push({
          name: 'ContestMySolutionsPage',
          params: {contest_id: this.contest_id}
        })
      } else {
        this.error_msg = (errors || []).join(', ')
      }
      this.loading = false
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      vm.loading = true
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (vm.currentContest && vm.currentContestIsInPast) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.currentContest.id}})
      }
      await vm.fetchAvailableCompilers()
      vm.loading = false
    })
  },
}
</script>

<style lang="scss" scoped>

</style>