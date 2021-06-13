<template>
  <div v-if="!!error_msg" class="alert alert-danger" role="alert">
    {{ error_msg }}
  </div>
  <v-form @submit="saveContest" :validation-schema="schema" class="mb-3">
    <div>
      <label>Название</label>
      <v-field v-model="name" class="form-control" name="name"/>
      <error-message name="name"></error-message>
    </div>
    <div>
      <label>Описание</label>
      <v-field v-model="description" as="textarea" class="form-control" name="description"/>
      <error-message name="description"></error-message>
    </div>
    <div>
      <label>Дата начала</label>
      <v-field v-model="startDateTimeUTC" class="form-control" type="datetime-local" name="startDateTimeUTC"/>
      <error-message name="startDateTimeUTC"></error-message>
    </div>
    <div>
      <label>Продолжительность в минутах</label>
      <v-field v-model="durationInMinutes" class="form-control" type="number" name="durationInMinutes"/>
      <error-message name="durationInMinutes"></error-message>
    </div>
    <div>
      <label>Разрешены виртуальные соревнования</label>
      <v-field v-model="areVirtualContestsAvailable" type="checkbox" value="1" name="areVirtualContestsAvailable"/>
      <error-message name="areVirtualContestsAvailable"></error-message>
    </div>
    <div>
      <label>Набор правил</label>
      <v-field class="form-control" v-model="rulesSetId" as="select" name="rulesSetId">
        <option :value="set.id" v-for="set of availableRuleSets">{{ set.name }}</option>
      </v-field>
      <error-message name="rulesSetId"></error-message>
    </div>
    <div class="row">
      <div class="col">
        <div>
          <label>Изображение</label>
          <v-field v-model="image" class="form-control" name="image" type="file"/>
          <error-message name="image"></error-message>
        </div>
      </div>
      <div class="col" v-if="!!dataUrl">
        <img class="img-fluid" :src="dataUrl"/>
      </div>
    </div>
    <tasks-selector-component :tasks="sortedTasks" @update:tasks="updateEvent"></tasks-selector-component>
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

      schema: Yup.object({
        name: Yup.string().required().nullable(),
        description: Yup.string().required().nullable(),
      })
    }
  },
  methods: {
    ...mapActions(['getContestById', 'fetchAvailableRuleSets', 'fetchRunningContests', 'fetchAvailableContests', 'fetchParticipatingContests', 'fetchCurrentUserContestsList']),
    async updateFields() {
      await this.fetchAvailableRuleSets();
      let post = await this.getContestById(this.contest_id)
      this.name = (post?.localizers || [])[0]?.name || null
      this.description = (post?.localizers || [])[0]?.description || null
      this.startDateTimeUTC = post?.startDateTimeUTC || null
      this.durationInMinutes = post?.durationInMinutes || null
      this.areVirtualContestsAvailable = post?.areVirtualContestsAvailable
      this.isPublic = +post?.isPublic === 1
      this.rulesSetId = post?.rulesSetId || null
      this.image = post?.image || null
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
      tmp_form.append($('<input type="hidden"/>').attr('name', 'isPublic').val(this.isPublic))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'startDateTimeUTC').val(this.startDateTimeUTC))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'durationInMinutes').val(this.durationInMinutes))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'areVirtualContestsAvailable').val(this.areVirtualContestsAvailable))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'rulesSetId').val(this.rulesSetId))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][culture]').val('ru'))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][description]').val(this.description))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][name]').val(this.name))
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
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'availableRuleSets']),
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
      await vm.updateFields()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>