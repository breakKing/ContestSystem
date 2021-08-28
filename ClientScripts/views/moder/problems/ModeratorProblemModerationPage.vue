<template>
  <div class="container">
    <div v-if="!!error_msg" class="alert alert-danger" role="alert">
          {{ error_msg }}
    </div>
    <div class="row p-3">
      <div class="col">
        <h2 style="font-weight: bold;">{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.name }}</h2>
        <h4 style="color: #4998AB;">Автор: {{ currentModeratingProblem && currentModeratingProblem.creator && currentModeratingProblem.creator.userName }}</h4>
      </div>
    </div>
    <div class="row p-3">
      <div class="col">
        <p class="semi-bold-text">Условие:</p>
        <p>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.description }}</p>
      </div>
    </div>
    <div class="row p-3">
      <div class="col">
        <span>Блок описания входных данных:</span>
        <p>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.inputBlock }}</p><br>
        <span>Блок описания выходных данных:</span>
        <p>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.outputBlock }}</p>
      </div>
    </div>
    <div class="row p-3">
      <div class="col">
        <span>Ограничение по времени: </span>
        <span class="semi-bold-text">{{ formatted_time_limit }}</span><br>
        <span>Ограничение по памяти: </span>
        <span class="semi-bold-text">{{ formatted_memory_limit }}</span><br>
        <span>Механизм проверки: </span>
        <span class="semi-bold-text">{{ currentModeratingProblem && currentModeratingProblem.checker 
          && currentModeratingProblem.checker.name }}</span><br>
        <span v-if="currentModeratingProblem && currentModeratingProblem.isPublic">Публичная</span>
        <span v-else><span class="semi-bold-text">Не</span> публичная</span>
      </div>
    </div>
    <div class="row p-3">
      <div class="col">
        <p>Тесты:</p>
        <template v-if="sortedTests && +sortedTests.length !== 0">
          <table class="table-main">
            <thead>
              <tr>
                <th>Входные данные</th>
                <th>Ожидаемый ответ</th>
                <th>Очки</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="test of sortedTests">
                <td>{{ test.input }}</td>
                <td>{{ test.answer }}</td>
                <td>{{ test.availablePoints }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </div>
    </div>
    <div class="row p-3">
      <div class="col">
        <p>Примеры:</p>
        <template v-if="sortedExamples && +sortedExamples.length !== 0">
          <table class="table-main">
            <thead>
              <tr>
                <th>Входные данные</th>
                <th>Выходные данные</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="example of sortedExamples">
                <td>{{ example.inputText }}</td>
                <td>{{ example.outputText }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </div>
    </div>
    <div class="row p-3">
      <div class="col">
        <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
          <div>
            <span>Комментарий</span>
            <v-field v-model="message" as="textarea" class="form-control" name="message"/>
            <error-message name="message"></error-message>
          </div>
          <div class="mt-3">
            <span>Статус</span>
            <v-field v-model="current_status" as="select" class="form-control" name="current_status">
              <option :value="approvalStatuses.NotModeratedYet">Не проверено</option>
              <option :value="approvalStatuses.Rejected">Отклонено</option>
              <option :value="approvalStatuses.Accepted">Утверждено</option>
            </v-field>
            <error-message name="current_status"></error-message>
          </div>
          <div class="mt-4">
            <button @click.prevent="deleteEntity" type="button" class="btn btn-danger">Удалить</button>
            <button type="submit" class="btn btn-primary ms-2">Сохранить</button>
          </div>
        </v-form>
      </div>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import * as Yup from "yup";
import {ErrorMessage, Field, Form} from "vee-validate";
import * as _ from 'lodash'

export default {
  name: "ModeratorProblemModerationPage",
  props: ['problem_id'],
  computed: {
    ...mapGetters(['currentUser', 'approvalStatuses', 'getFormattedMemory', 'getFormattedTime']),
    ...mapGetters('moder_problems', [
      'currentModeratingProblem',
      'currentModeratingProblemLocalizer',
    ]),
    sortedTests() {
      return _.orderBy((this.currentModeratingProblem?.tests || []), ['number'], ['asc'])
    },
    sortedExamples() {
      return _.orderBy((this.currentModeratingProblem?.examples || []), ['number'], ['asc'])
    },
    formatted_memory_limit() {
      return this.getFormattedMemory(this.currentModeratingProblem?.memoryLimitInBytes)
    },
    formatted_time_limit() {
      return this.getFormattedTime(this.currentModeratingProblem?.timeLimitInMilliseconds)
    }
  },
  methods: {
    ...mapActions('moder_problems', [
      'changeCurrentProblem',
      'fetchProblemsToModerate',
      'fetchRejectedProblems',
      'fetchApprovedProblems',
      'moderateProblem',
    ]),
    ...mapActions(['deleteTask']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteTask(this.problem_id)
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async submitEntity() {
      this.error_msg = ''
      let {status, errors} = await this.moderateProblem({
        problem_id: this.problem_id,
        request_body: {
          problemId: +this.problem_id,
          approvalStatus: +this.current_status,
          approvingModeratorId: this.currentUser.id,
          moderationMessage: this.message,
        }
      })
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async fetchDataAndGoToList() {
      await this.fetchProblemsToModerate(true)
      await this.fetchRejectedProblems(true)
      await this.fetchApprovedProblems(true)
      await this.changeCurrentProblem({force: false, problem_id: null})
      await this.$router.push({
        name: 'ModeratorNotModeratedProblemsPage'
      })
    }
  },
  data() {
    return {
      error_msg: '',
      message: '',
      current_status: null,
      schema: Yup.object({
        message: Yup.string().nullable(),
        current_status: Yup.number().required().nullable(),
      })
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentProblem({force: false, problem_id: vm.problem_id})
      vm.message = vm.currentModeratingProblem?.moderationMessage
      vm.current_status = +vm.currentModeratingProblem?.approvalStatus
      vm.error_msg = ''
    })
  },
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
}
</script>

<style scoped>
  p, span {
    font-size: 1.2rem;
  }
  .semi-bold-text {
    font-weight: 600;
  }
</style>