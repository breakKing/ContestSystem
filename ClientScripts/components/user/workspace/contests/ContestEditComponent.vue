<template>
  <div v-if="!!error_msg" class="alert alert-danger" role="alert">
    {{ error_msg }}
  </div>
  <v-form @submit="saveContest" :validation-schema="schema" class="mb-3">
    <div>
      <label class=" fs-4">Название</label>
      <v-field v-model="name" class="form-control" name="name"/>
      <error-message name="name"></error-message>
    </div>
    <div>
      <label class=" fs-4">Описание</label>
      <v-field v-model="description" as="textarea" class="form-control" name="description"/>
      <error-message name="description"></error-message>
    </div>
    <div>
      <label class=" fs-4">Дата начала</label>
      <v-field v-model="startDateTimeUTC" class="form-control" type="datetime-local" name="startDateTimeUTC"/>
      <error-message name="startDateTimeUTC"></error-message>
    </div>
    <div>
      <label class="fs-4">Продолжительность в минутах</label>
      <v-field v-model="durationInMinutes" class="form-control" type="number" name="durationInMinutes"/>
      <error-message name="durationInMinutes"></error-message>
    </div>
    <div>
        <v-field v-model="areVirtualContestsAvailable" class="custom-checkbox" id="areVirtualContestsAvailable" type="checkbox" :value="true" :uncheckedValue="false" name="areVirtualContestsAvailable" />
        <label class=" fs-4" for="areVirtualContestsAvailable">Разрешены виртуальные соревнования</label>
        <error-message name="areVirtualContestsAvailable"></error-message>
    </div>
    <div>
      <label class=" fs-4">Набор правил</label>
      <v-field class="form-control" v-model="rulesSetId" as="select" name="rulesSetId">
          <option :value="set.id" v-for="set of availableRuleSetsForContest">{{ set.name }} {{ shouldRulesSetBeRemarked(set) ? '*' : '' }}</option>
      </v-field>
      <error-message name="rulesSetId"></error-message>
      <p v-if="unavailableRulesSetsInFutureExists">* Данный набор правил более недоступен. Однако Вы можете использовать его для этого соревнования до тех пор, пока не замените его.</p>
    </div>
    <div class="row">
      <div class="col">
        <div>
          <label class=" fs-4">Изображение</label>
          <v-field v-model="image" class="form-control" name="image" type="file"/>
          <error-message name="image"></error-message>
        </div>
      </div>
      <div class="col" v-if="!!dataUrl">
        <img class="img-fluid" :src="dataUrl"/>
      </div>
    </div>
    <tasks-selector-component :tasks="sortedTasks" :availableTasksForContest="availableTasksForContest" @update:tasks="updateEvent"></tasks-selector-component>
    <button type="submit" class="btn btn-primary">Сохранить</button>
  </v-form>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TasksSelectorComponent from "./TasksSelectorComponent";
import * as _ from 'lodash'
import * as Yup from 'yup';
import {ErrorMessage, Field, Form} from "vee-validate";
import alphabet from 'alphabet'
import $ from "jquery";
import moment from "moment";

export default {
  name: "ContestEditComponent",
  components: {
    TasksSelectorComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  props: ['contest_id'],
  data() {
    return {
      error_msg: '',

      isPublic: null,
      startDateTimeUTC: null,
      durationInMinutes: null,
      areVirtualContestsAvailable: null,
      rulesSetId: null,
      tasks: [],
      description: '',
      image: '',
      name: '',
      startedRulesSet: null,
      startedProblems: [],

      schema: Yup.object({
        name: Yup.string('Название должно быть строкой').nullable().required('Название это обязательное поле'),
        description: Yup.string('Описание должно быть строкой').nullable().required('Описание это обязательное поле')
      })
    }
  },
  methods: {
    ...mapActions(['getContestById', 'getRuleSet', 'fetchAvailableRuleSets', 'fetchRunningContests', 'fetchAvailableContests', 'fetchParticipatingContests', 'fetchCurrentUserContestsList', 'fetchAvailableTasks']),
    async updateFields() {
      await this.fetchAvailableRuleSets()
      await this.fetchAvailableTasks()
      let contest = await this.getContestById(this.contest_id)
      this.name = (contest?.localizers || [])[0]?.name || null
      this.description = (contest?.localizers || [])[0]?.description || null
      this.startDateTimeUTC = contest?.startDateTimeUTC || null
      this.durationInMinutes = contest?.durationInMinutes || null
      this.areVirtualContestsAvailable = contest?.areVirtualContestsAvailable || false
      this.isPublic = contest?.isPublic || false
      this.rulesSetId = contest?.rulesSetId || null
      this.startedRulesSet = contest?.rules || null
      this.image = contest?.image || null
      this.tasks = contest?.problems || []
      this.startedProblems = contest?.problems || []
    },
    updateEvent(data) {
      if (data.type === 'add') {
        this.tasks.push({
          contestId: this.contest_id,
          letter: data.letter,
          problemId: null
        })
      } else if (data.type === 'delete') {
        this.tasks = _.filter(this.tasks, (t) => t.letter !== data.letter)
        _.each(this.tasks, (t, i) => {
          t.letter = alphabet.upper[i]
        })
      } else if (data.type === 'changed') {
        this.tasks = _.map(this.tasks, (t) => {
          if (t.letter === data.letter) {
            t.problemId = data.problemId
          }
          return t
        })
      }
    },
    async saveContest() {
      if (!this.currentUser) {
        this.error_msg = 'С вашими авторизационными данными что-то не так'
        return
      }
      let tmp_form = $('<form enctype="multipart/form-data"></form>');
      tmp_form.append($('<input type="hidden"/>').attr('name', 'id').val(this.contest_id))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'creatorUserId').val(this.currentUser.id))
      //tmp_form.append($('<input type="hidden"/>').attr('name', 'isPublic').val(this.isPublic)) TODO: реализовать приватность контестов
      tmp_form.append($('<input type="hidden"/>').attr('name', 'isPublic').val(true))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'startDateTimeUTC').val(moment(this.startDateTimeUTC).utc().format()))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'durationInMinutes').val(this.durationInMinutes))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'areVirtualContestsAvailable').val(this.areVirtualContestsAvailable))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'rulesSetId').val(this.rulesSetId))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][culture]').val('ru'))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][description]').val(this.description))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][name]').val(this.name))
      if (!this.sortedTasks || this.sortedTasks.length <= 0) {
        tmp_form.append($('<input type="hidden"/>').attr('name', 'problems').val(null))
      }
      _.each(this.sortedTasks, (problem, idx) => {
        tmp_form.append($('<input type="hidden"/>').attr('name', `problems[${idx}][problemId]`).val(problem.problemId))
        tmp_form.append($('<input type="hidden"/>').attr('name', `problems[${idx}][letter]`).val(problem.letter))
      })
      tmp_form.append($('[name="image"]').clone())
      let data = new FormData(tmp_form[0]);

      let url;
      let method;
      if (this.contest_id) {
        url = `/api/contests/edit-contest/${this.contest_id}`
        method = 'PUT'
      } else {
        url = `/api/contests/add-contest`
        method = 'POST'
      }

      let result = await $.ajax({
        url,
        data,
        processData: false,
        contentType: false,
        type: method
      })
      if (result.status) {
        this.error_msg = ''

        await this.fetchRunningContests(true)
        await this.fetchAvailableContests(true)
        await this.fetchParticipatingContests(true)
        await this.fetchCurrentUserContestsList(true)
        await this.$router.push({name: 'WorkSpaceMyPendingContestsPage'})

      } else {
        this.error_msg = (result.errors || []).join(', ')
      }
    },
    shouldRulesSetBeRemarked(rulesSet) {
      let remark = false
      if (rulesSet) {
        if ((rulesSet.author?.id != this.currentUser.id && !rulesSet.isPublic) || rulesSet.isArchieved) {
          remark = true
        }
      }
      return remark
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'availableRuleSets', 'availableTasks']),
    availableRuleSetsForContest() {
      if (!this.startedRulesSet) {
        return this.availableRuleSets
      }
      return _.unionBy(this.availableRuleSets || [], [ this.startedRulesSet ], (rs) => rs.id)
    },
    availableTasksForContest() {
      let startedTasks = _.map(this.startedProblems || [], (sp, i) => sp.problem)
      return _.unionBy(this.availableTasks || [], startedTasks || [], (t) => t.id)
    },
    unavailableRulesSetsInFutureExists() {
      return _.reduce(this.availableRuleSetsForContest || [], (count, rs) => count += +this.shouldRulesSetBeRemarked(rs), 0) > 0
    },
    sortedTasks() {
      return _.sortBy(this.tasks, ['letter'])
    },
    dataUrl() {
      if (!this.image) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.image)) {
        const [file] = this.image
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.image
    },
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAvailableTasks()
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
    .row, .col{
        margin: 0;
        padding: 0;
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