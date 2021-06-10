﻿<template>
  <!--eslint-disable -->
  <div class="row">
    <div class="col">
      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <v-form @submit="saveTask" :validation-schema="schema">
        <div>
          <label class="text-light fs-3">Название</label>
          <v-field v-model="name" class="form-control" name="name"/>
          <error-message name="name"></error-message>
        </div>
        <div>
          <label class="text-light fs-3">Описание</label>
          <v-field v-model="description" class="form-control" name="description"/>
          <error-message name="description"></error-message>
        </div>
        <div>
          <label class="text-light fs-3">Входные данные</label>
          <v-field v-model="inputBlock" class="form-control" name="inputBlock"/>
          <error-message name="inputBlock"></error-message>
        </div>
        <div>
          <label class="text-light fs-3">Выходные данные</label>
          <v-field v-model="outputBlock" class="form-control" name="outputBlock"/>
          <error-message name="outputBlock"></error-message>
        </div>
        <div>
          <label class="text-light fs-3">Лимит занимаемого RAM (bytes)</label>
          <v-field v-model="memoryLimitInBytes" class="form-control" name="memoryLimitInBytes"/>
          <error-message name="memoryLimitInBytes"></error-message>
        </div>
        <div>
          <label class="text-light fs-3">Лимит времени выполнения (мсек)</label>
          <v-field v-model="timeLimitInMilliseconds" class="form-control" name="timeLimitInMilliseconds"/>
          <error-message name="timeLimitInMilliseconds"></error-message>
        </div>
        <div>
          <label class="text-light fs-3">Механизм проверки</label>
          <v-field v-model="checker" class="form-control" name="checker" as="select">
            <option v-for="checker of availableCheckers" :value="checker.id">{{ checker.name }}</option>
          </v-field>
          <error-message name="checker"></error-message>
        </div>
        <div>
          <v-field v-model="isPublic" class="custom-checkbox" id="isPublic" name="isPublic" type="checkbox" value="1"/>
          <label class="text-light fs-5" for="isPublic">Виден всем</label>
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


export default {
  name: "TaskEditComponent",
  props: ['task_id'],
  components: {
    ExamplesTableComponent,
    TestsTableComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
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

      schema: Yup.object({
        memoryLimitInBytes: Yup.number('Лимит занимаемого RAM это число').nullable().required('Лимит занимаемого RAM это обязательное поле'),
        timeLimitInMilliseconds: Yup.number('Лимит времени выполнения это число').nullable().required('Лимит времени выполнения это обязательное поле'),
        checker: Yup.number('Механизм проверки это число').required('Механизм проверки обязательно должен быть выбран'),

        name: Yup.string('Название это строка').required('Название это обязательное поле'),
        description: Yup.string('Описание это строка').required('Описание это обязательное поле'),
        inputBlock: Yup.string('Входные данные это строка').required('Входные данные это обязательное поле'),
        outputBlock: Yup.string('Выходные данные это строка').required('Выходные данные это обязательное поле'),
      })
    }
  },
  computed: {
    ...mapGetters(['availableCheckers', 'currentUser']),
    sortedTests() {
      return this.tests.sort((a, b) => a.number - b.number)
    },
    sortedExamples() {
      return this.examples.sort((a, b) => a.number - b.number)
    }
  },
  methods: {
    ...mapActions(['getTask', 'fetchAvailableCheckers']),
    async updateFields() {
      await this.fetchAvailableCheckers();
      let data = await this.getTask(this.task_id)
      this.memoryLimitInBytes = data?.memoryLimitInBytes
      this.timeLimitInMilliseconds = data?.timeLimitInMilliseconds
      this.isPublic = data?.isPublic
      this.checker = data?.checker.id
      this.name = data?.localizers[0]?.name
      this.description = data?.localizers[0]?.description
      this.inputBlock = data?.localizers[0]?.inputBlock
      this.outputBlock = data?.localizers[0]?.outputBlock
      this.tests = (data?.tests || [])
      this.examples = (data?.examples || [])
    },
    async saveTask() {
      let url;
      let method;
      if (this.task_id) {
        url = `/api/problems/edit-problem/${this.task_id}`
        method = 'put'

      } else {
        url = `/api/problems/add-problem`
        method = 'post'
      }
      let data = {
        id: (!!this.task_id ? this.task_id : null),
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
        TimeLimitInMilliseconds: this.TimeLimitInMilliseconds,
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
        await this.$router.push({name: 'WorkSpaceMyApprovedTasksPage'})
      } else {
        this.error_msg = (result.errors || []).join(', ')
      }
    },
    testsUpdated(changes) {
      if (changes.type === 'delete') {
        this.tests.splice(this.tests.indexOf(this.sortedTests[changes.index]), 1)
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
        this.examples.splice(this.examples.indexOf(this.sortedExamples[changes.index]), 1)
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
  },
  async created() {
    await this.updateFields()
  },
  watch: {
    async $route(to, from) {
      if (to.name === 'WorkSpaceEditTaskPage') {
        await this.updateFields()
      }
    }
  },

}
</script>

<style lang="scss" scoped>
div * {
  margin: 10px 0;
}

span[role=alert] {
  color: white;
}

form {
  padding: 10px;
  border: 1px solid blue;
  border-radius: 0 16px 16px 0;
  background-color: #0D6EFD;
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
  border-radius: 16px 0 16px 0;
}

button {
  padding: 5px 10px;
  background-color: #fff;
  border-radius: 16px 0 16px 0;
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