<template>
  <div class="row">
    <div class="col-12 col-md-3">
      <tasks-navigation-component :active_task_id="actualTaskId" :tasks="orderedTasks"
                                  :mapped_solutions="mapped_solutions"></tasks-navigation-component>
    </div>
    <div class="col">
      <router-link class="btn btn-info" :to="{name: 'ContestMySolutionsPage', params:{contest_id: contest_id}}">Мои
        попытки
      </router-link>

      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <h2>{{ problem_name }}</h2>
      <p><i>({{ problem?.timeLimitInMilliseconds }} мсек, {{ problem?.memoryLimitInBytes }} байт)</i></p>
      <p>{{ problem_description }}</p>
      <p>{{ problem_input }}</p>
      <p>{{ problem_output }}</p>
      <template v-if="sortedExamples.length > 0">
        <div class="row">
          <div class="col">Вход</div>
          <div class="col">Выход</div>
        </div>
        <div class="row" v-for="example of sortedExamples">
          <div class="col">{{ example.inputText }}</div>
          <div class="col">{{ example.outputText }}</div>
        </div>
      </template>

      <h2>Решение</h2>
      <v-form @submit="onSubmitSolution" :validation-schema="solutionSchema" class="mb-3">
        <div>
          <v-field :disabled="loading" as="textarea" v-model="code" class="form-control code-input" name="code"/>
          <error-message name="code"></error-message>
        </div>
        <div>
          <label>Компилятор</label>
          <v-field :disabled="loading" as="select" v-model="compiler" class="form-control" name="compiler">
            <option :value="cpm.guid" v-for="cpm of availableCompilers">{{ cpm.name }}</option>
          </v-field>
          <error-message name="compiler"></error-message>
        </div>
        <div class="form-group mt-3">
          <button class="btn btn-primary" :disabled="loading" type="submit">Отправить</button>
        </div>
      </v-form>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TasksNavigationComponent from "./TasksNavigationComponent";
import * as _ from "lodash";
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";
import 'codemirror'

export default {
  name: "TaskComponent",
  components: {
    TasksNavigationComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  props: ['contest_id', 'task_id'],
  data() {
    return {
      loading: false,
      problem: null,
      compiler: null,
      error_msg: '',
      code: '',

      solutionSchema: Yup.object({
        compiler: Yup.string().required(),
        code: Yup.string().required(),
      })
    }
  },
  computed: {
    ...mapGetters([
      'currentUser',
      'currentContest',
      'currentContestSolutionsForCurrentUser', // тут инфа о попытках каждого таска(для навигатора)
      'availableCompilers',
      'currentContestIsInPast',
    ]),
    orderedTasks() {
      return _.sortBy((this.currentContest?.problems || []), ['letter'])
    },
    actualTaskId() {
      if (this.task_id) {
        return this.task_id
      }
      return _.first(this.orderedTasks).id
    },
    localizer() {
      return _.find((this.problem?.localizers || []), (l) => l.culture === 'ru')
    },
    problem_name() {
      return this.localizer?.name
    },
    problem_description() {
      return this.localizer?.description
    },
    problem_input() {
      return this.localizer?.inputBlock
    },
    problem_output() {
      return this.localizer?.outputBlock
    },
    sortedExamples() {
      return _.sortBy((this.problem?.examples || []), ['number'])
    },
    mapped_solutions() {
      return _.groupBy(this.currentContestSolutionsForCurrentUser, (s) => +s.problem.id)
    }
  },
  methods: {
    ...mapActions(['getTask', 'changeCurrentContest', 'fetchAvailableCompilers', 'compileSolution', 'runSolutionTests']),
    async onSubmitSolution() {
      this.loading = true
      let {data: solution_id, status, errors} = await this.compileSolution({
        code: this.code,
        compilerGUID: this.compiler,
        contestId: this.contest_id,
        userId: this.currentUser.id,
        problemId: this.actualTaskId,
      })
      if (solution_id) {
        this.error_msg = ''
        // запустили и пофиг на результат
        this.runSolutionTests(solution_id)

        await this.$router.push({
          name: 'ContestMySolutionsPage',
          params: {solution_id: solution_id, contest_id: this.contest_id}
        })

      } else {
        this.error_msg = (errors || []).join(', ')
      }
      this.loading = false
    }
  },
  mounted() {
    CodeMirror.fromTextArea(document.querySelector('.code-input'), {
      lineNumbers: true,
    })
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (vm.currentContest && vm.currentContestIsInPast) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.currentContest.id}})
      }
      vm.problem = await vm.getTask(vm.actualTaskId)
      await vm.fetchAvailableCompilers()
      vm.loading = false
    })
  },
}
</script>

<style lang="scss" scoped>

</style>