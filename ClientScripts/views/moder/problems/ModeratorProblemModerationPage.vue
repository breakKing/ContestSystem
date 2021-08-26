<template>
  <div class="row p-3">
    <div class="col">

      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <div class="row">
        <div class="col">
          <div>
            <h2>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.name }}</h2>
          </div>
          <div>
            <p>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.description }}</p>
          </div>
          <div class="row">
            <div class="col">
              <span class="text-light fs-3">Входные данные</span>
              <p>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.inputBlock }}</p>
            </div>
            <div class="col">
              <span class="text-light fs-3">Выходные данные</span>
              <p>{{ currentModeratingProblemLocalizer && currentModeratingProblemLocalizer.outputBlock }}</p>
            </div>
          </div>
          <div class="row">
            <div class="col">
              <span class="text-light fs-3">Лимит занимаемого RAM (bytes)</span>
              <span>{{ currentModeratingProblem && currentModeratingProblem.memoryLimitInBytes }}</span>
            </div>
            <div class="col">
              <span class="text-light fs-3">Лимит времени выполнения (мсек)</span>
              <span>{{ currentModeratingProblem && currentModeratingProblem.timeLimitInMilliseconds }}</span>
            </div>
          </div>
          <div class="row">
            <div class="col">
              <span class="text-light fs-3">Механизм проверки:</span>
              <span
                  class="text-light fs-3">{{
                  currentModeratingProblem && currentModeratingProblem.checker && currentModeratingProblem.checker.name
                }} ({{
                  currentModeratingProblem && currentModeratingProblem.checker && currentModeratingProblem.checker.author && currentModeratingProblem.checker.author.fullName
                }})</span>
            </div>
            <div class="col">
              <span class="text-light fs-5">Виден всем: {{
                  currentModeratingProblem && currentModeratingProblem.isPublic ? 'Да' : 'Нет'
                }}</span>
            </div>
          </div>
          <div>
            <h4>Тесты</h4>
            <table>
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
          </div>
          <div>
            <h4>Примеры</h4>
            <table>
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
          </div>
        </div>
      </div>


      <div class="row">
        <div class="col">
          <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
            <div>
              <span>Комментарий</span>
              <v-field v-model="message" as="textarea" class="form-control" name="message"/>
              <error-message name="message"></error-message>
            </div>
            <div>
              <span>Статус</span>
              <v-field v-model="current_status" as="select" class="form-control" name="current_status">
                <option :value="approvalStatuses.NotModeratedYet">Не проверено</option>
                <option :value="approvalStatuses.Rejected">Отклонено</option>
                <option :value="approvalStatuses.Accepted">Утверждено</option>
              </v-field>
              <error-message name="current_status"></error-message>
            </div>
            <div class="mt-2">
              <button @click.prevent="deleteEntity" type="button" class="btn btn-danger">Удалить</button>
              <button type="submit" class="btn btn-primary ms-2">Сохранить</button>
            </div>
          </v-form>
        </div>
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
    ...mapGetters(['currentUser', 'approvalStatuses']),
    ...mapGetters('moder_problems', [
      'currentModeratingProblem',
      'currentModeratingProblemLocalizer',
    ]),
    sortedTests() {
      return _.orderBy((this.currentModeratingProblem?.tests || []), ['number'], ['asc'])
    },
    sortedExamples() {
      return _.orderBy((this.currentModeratingProblem?.examples || []), ['number'], ['asc'])
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

</style>