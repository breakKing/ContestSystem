<template>
  <!--eslint-disable -->
  <div class="row">
    <div class="col">
      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <v-form @submit="saveTask" :validation-schema="schema">
        <div>
          <label class="fs-4">Название</label>
          <v-field v-model="name" class="form-control" name="name"/>
          <error-message name="name"></error-message>
        </div>
        <div>
          <label class="fs-4">Описание</label>
          <v-field v-model="description" name="description" type="hidden"/>
          <quill-editor ref="quill_editor_description" theme="snow" v-model:content="description" contentType="html"
                        toolbar="full" class="form-control"></quill-editor>
          <error-message name="description"></error-message>
        </div>
        <div>
          <label class="fs-4">Входные данные</label>
          <v-field v-model="inputBlock" name="inputBlock" type="hidden"/>
          <quill-editor ref="quill_editor_inputBlock" theme="snow" v-model:content="inputBlock" contentType="html"
                        toolbar="full" class="form-control"></quill-editor>
          <error-message name="inputBlock"></error-message>
        </div>
        <div>
          <label class="fs-4">Выходные данные</label>
          <v-field v-model="outputBlock" name="outputBlock" type="hidden"/>
          <quill-editor ref="quill_editor_outputBlock" theme="snow" v-model:content="outputBlock" contentType="html"
                        toolbar="full" class="form-control"></quill-editor>
          <error-message name="outputBlock"></error-message>
        </div>
        <div>
          <label class="fs-4">Лимит занимаемого RAM (bytes)</label>
          <v-field v-model="memoryLimitInBytes" class="form-control" name="memoryLimitInBytes"/>
          <error-message name="memoryLimitInBytes"></error-message>
        </div>
        <div>
          <label class="fs-4">Лимит времени выполнения (мсек)</label>
          <v-field v-model="timeLimitInMilliseconds" class="form-control" name="timeLimitInMilliseconds"/>
          <error-message name="timeLimitInMilliseconds"></error-message>
        </div>
        <div>
          <label class="fs-4">Механизм проверки</label>
          <v-field v-model="checker" class="form-control" name="checker" as="select">
            <option v-for="checker of availableCheckersForProblem" :value="checker.id">{{ checker.name }}
              {{ shouldCheckerBeRemarked(checker) ? '*' : '' }}
            </option>
          </v-field>
          <error-message name="checker"></error-message>
          <p v-if="unavailableCheckersInFutureExists">* Данный механизм проверки более недоступен. Однако Вы можете
            использовать его для этой задачи до тех пор, пока не замените его.</p>
        </div>
        <div>
          <v-field v-model="isPublic" class="custom-checkbox" id="isPublic" name="isPublic" type="checkbox"
                   :value="true" :uncheckedValue="false"/>
          <label class="fs-4" for="isPublic">Виден всем</label>
          <error-message name="isPublic"></error-message>
        </div>
        <div>
          <tests-table-component :tests="sortedTests" @update:tests="testsUpdated"></tests-table-component>
        </div>
        <div>
          <examples-table-component :examples="sortedExamples" @update:examples="examplesUpdated">
          </examples-table-component>
        </div>
        <button type="submit">Сохранить</button>
      </v-form>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import {Field, Form, ErrorMessage} from "vee-validate";
import * as Yup from 'yup';
import {mapActions, mapGetters} from "vuex";
import TestsTableComponent from "./TestsTableComponent";
import * as _ from 'lodash'
import ExamplesTableComponent from "./ExamplesTableComponent";
import {QuillEditor} from "@vueup/vue-quill";


export default {
  name: "TaskEditComponent",
  props: ['task_id'],
  components: {
    ExamplesTableComponent,
    TestsTableComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
    QuillEditor
  },
  data() {
    return {
      error_msg: '',

      memoryLimitInBytes: null,
      timeLimitInMilliseconds: null,
      isPublic: null,
      checker: null,
      tests: [],
      examples: [],
      name: '',
      description: '',
      inputBlock: '',
      outputBlock: '',
      startedChecker: null,

      schema: Yup.object({
        memoryLimitInBytes: Yup.number('Лимит занимаемого RAM это число').nullable().required('Лимит занимаемого RAM это обязательное поле'),
        timeLimitInMilliseconds: Yup.number('Лимит времени выполнения это число').nullable().required('Лимит времени выполнения это обязательное поле'),
        checker: Yup.number('Механизм проверки это число').required('Механизм проверки обязательно должен быть выбран'),

        name: Yup.string('Название это строка').nullable().required('Название это обязательное поле'),
        description: Yup.string('Описание это строка').nullable().required('Описание это обязательное поле'),
        inputBlock: Yup.string('Входные данные это строка').nullable().required('Входные данные это обязательное поле'),
        outputBlock: Yup.string('Выходные данные это строка').nullable().required('Выходные данные это обязательное поле'),
      })
    }
  },
  computed: {
    ...mapGetters(['availableCheckers', 'currentUser']),
    availableCheckersForProblem() {
      if (!this.startedChecker) {
        return this.availableCheckers
      }
      return _.unionBy(this.availableCheckers || [], [this.startedChecker], (c) => c.id)
    },
    unavailableCheckersInFutureExists() {
      return _.reduce(this.availableCheckersForProblem || [], (count, c) => count + +this.shouldCheckerBeRemarked(c), 0) > 0
    },
    sortedTests() {
      return _.sortBy((this.tests || []), (t) => t.number)
    },
    sortedExamples() {
      return _.sortBy((this.examples || []), (t) => t.number)
    },
    testsPointsSumIsValid() {
      return _.reduce((this.tests || []), (sum, t) => +t.availablePoints + sum, 0) === 100
    }
  },
  methods: {
    ...mapActions(['getTask', 'fetchAvailableCheckers', 'fetchCurrentUserTasks', 'fetchAvailableTasks']),
    async updateFields() {
      await this.fetchAvailableCheckers();
      let data = await this.getTask(this.task_id)
      this.memoryLimitInBytes = data?.memoryLimitInBytes || null
      this.timeLimitInMilliseconds = data?.timeLimitInMilliseconds || null
      this.isPublic = data?.isPublic || false
      this.checker = data?.checker?.id || null
      this.startedChecker = data?.checker || null
      this.name = (data?.localizers || [])[0]?.name || null
      this.description = (data?.localizers || [])[0]?.description || null
      this.inputBlock = (data?.localizers || [])[0]?.inputBlock || null
      this.outputBlock = (data?.localizers || [])[0]?.outputBlock || null
      this.$refs.quill_editor_description.setHTML(this.description)
      this.$refs.quill_editor_inputBlock.setHTML(this.inputBlock)
      this.$refs.quill_editor_outputBlock.setHTML(this.outputBlock)
      this.tests = (data?.tests || [])
      this.examples = (data?.examples || [])
    },
    async saveTask() {
      if (!this.testsPointsSumIsValid) {
        this.error_msg = 'Общая сумма очков за все задачи должна составлять 100'

        return
      }
      let url;
      let method;
      if (this.task_id) {
        url = `/api/workspace/problems/${this.task_id}`
        method = 'put'

      } else {
        url = `/api/workspace/problems`
        method = 'post'
      }
      let data = {
        id: this.task_id,
        creatorId: this.currentUser.id,
        localizers: [
          {
            culture: 'ru',
            name: this.name,
            description: this.description,
            inputBlock: this.inputBlock,
            outputBlock: this.outputBlock,
          }
        ],
        memoryLimitInBytes: this.memoryLimitInBytes,
        timeLimitInMilliseconds: this.timeLimitInMilliseconds,
        isPublic: this.isPublic,
        checkerId: this.checker,
        tests: this.tests,
        examples: this.examples,
      }
      let result = false
      try {
        result = await axios[method](url, data)
      } catch (e) {
        console.error(e)
        this.error_msg = 'Не удалось сохранить'
      }
      if (result.status) {
        this.error_msg = '';
        await this.fetchCurrentUserTasks(true)
        await this.fetchAvailableTasks(true)
        if (this.currentUser?.limits?.problems) {
          await this.$router.push({name: 'WorkSpaceMyPendingTasksPage'})
        } else {
          await this.$router.push({name: 'WorkSpaceMyApprovedTasksPage'})
        }
      } else {
        this.error_msg = (result.errors || []).join(', ')
      }
    },
    testsUpdated(changes) {
      if (changes.type === 'delete') {
        let deleted = this.sortedTests[changes.index]
        let deleted_index = this.tests.indexOf(deleted)
        _.forEach(_.filter(this.tests, (t) => +t.number > +deleted.number), (t) => {
          t.number--
        })
        this.tests.splice(deleted_index, 1)
      } else if (changes.type === 'up-order') {
        let changed_test = this.sortedTests[changes.index]
        if (changed_test.number <= 1) {
          return
        }
        _.forEach(_.filter(this.tests, (t) => +t.number === (changed_test.number - 1)), (t) => {
          t.number++
        })
        changed_test.number--
      } else if (changes.type === 'down-order') {
        let changed_test = this.sortedTests[changes.index]
        if (changes.index === this.sortedTests.length - 1) {
          return;
        }
        _.forEach(_.filter(this.tests, (t) => +t.number === (changed_test.number + 1)), (t) => {
          t.number--
        })
        changed_test.number++
      } else if (changes.type === 'change') {
        let changed_test = this.sortedTests[changes.index]
        Object.assign(changed_test, changes.value)
      } else if (changes.type === 'add') {
        this.tests.push({
          number: this.tests.length + 1
        })
      }
    },
    examplesUpdated(changes) {
      if (changes.type === 'delete') {
        let deleted = this.sortedExamples[changes.index]
        let deleted_index = this.examples.indexOf(deleted)
        _.forEach(_.filter(this.examples, (t) => +t.number > +deleted.number), (t) => {
          t.number--
        })
        this.examples.splice(deleted_index, 1)
      } else if (changes.type === 'up-order') {
        let changed_example = this.sortedExamples[changes.index]
        if (changed_example.number <= 1) {
          return
        }
        _.forEach(_.filter(this.examples, (t) => +t.number === (changed_example.number - 1)), (t) => {
          t.number++
        })
        changed_example.number--
      } else if (changes.type === 'down-order') {
        let changed_example = this.sortedExamples[changes.index]
        if (changes.index === this.sortedExamples.length - 1) {
          return;
        }
        _.forEach(_.filter(this.examples, (t) => +t.number === (changed_example.number + 1)), (t) => {
          t.number--
        })
        changed_example.number++
      } else if (changes.type === 'change') {
        let changed_example = this.sortedExamples[changes.index]
        Object.assign(changed_example, changes.value)
      } else if (changes.type === 'add') {
        this.examples.push({
          number: this.examples.length + 1
        })
      }
    },
    shouldCheckerBeRemarked(checker) {
      let remark = false
      if (checker) {
        if ((checker.author?.id != this.currentUser.id && !checker.isPublic) || checker.isArchieved) {
          remark = true
        }
      }
      return remark
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.updateFields()
    })
  },
}
</script>

<style lang="scss" scoped>
div * {
  margin: 5px;
  color: #04295E;
}

span[role=alert] {
  color: red;
}

form {
  padding: 10px;
}

.custom-checkbox {
  position: absolute;
  z-index: -1;
  opacity: 0;
}

.custom-checkbox + label {
  display: inline-flex;
  align-items: center;
  user-select: none;
}

.custom-checkbox + label::before {
  content: '';
  display: inline-block;
  width: 1em;
  height: 1em;
  flex-shrink: 0;
  flex-grow: 0;
  border: 1px solid #adb5bd;
  border-radius: 0.25em;
  margin-right: 0.5em;
  background-repeat: no-repeat;
  background-position: center center;
  background-size: 50% 50%;
}

.custom-checkbox:checked + label::before {
  background-color: #0b76ef;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%23fff' d='M6.564.75l-3.59 3.612-1.538-1.55L0 4.26 2.974 7.25 8 2.193z'/%3e%3c/svg%3e");
}

.form-control {
  border-radius: 16px;
}

button {
  padding: 5px 10px;
  background-color: #fff;
  border-radius: 16px;
  border: 1px solid blue;

  &:hover {
    background-color: #0b76ef;
    color: white;
  }
}

.form-control::-webkit-input-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control::-moz-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control:-moz-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control:-ms-input-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control:focus::-webkit-input-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.form-control:focus::-moz-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.form-control:focus:-moz-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.form-control:focus:-ms-input-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}
</style>