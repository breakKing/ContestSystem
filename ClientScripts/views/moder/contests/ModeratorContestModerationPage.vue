<template>
  <div v-if="!!error_msg" class="alert alert-danger" role="alert">
    {{ error_msg }}
  </div>
  <div class="row">
    <div class="col">
      <div class="row">
        <div class="col">
          <div>
            <h2>{{ currentModeratingContestLocalizer?.name }}</h2>
          </div>
        </div>
        <div class="col-4" v-if="!!dataUrl">
          <img class="img-fluid" alt="картинка" :src="dataUrl"/>
        </div>
      </div>
      <div>
        <p>{{ currentModeratingContestLocalizer?.description }}</p>
      </div>
      <div>
        <span>Дата начала</span>
        <span>{{ formatted_start_date }}</span>
      </div>
      <div>
        <span>Продолжительность в минутах</span>
        <span>{{ currentModeratingContest?.durationInMinutes }}</span>
      </div>
      <div>
        <span>Набор правил</span>
        <span>{{ currentModeratingContest?.rules?.name }}</span>
      </div>
      <div class="row" v-for="problem of sortedTasks">
        <div class="col">
          <span>{{ problem.letter }} {{ problem.problem.localizedName }}</span>
        </div>
      </div>

      <div class="row">
        <div class="col">
          <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
            <div>
              <span>Комментарий</span>
              <v-field v-model="message" aas="textarea" class="form-control" name="message"/>
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
            <button @click.prevent="deleteEntity" type="button" class="btn btn-danger">Удалить</button>
            <button type="submit" class="btn btn-primary">Сохранить</button>
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
import * as _ from "lodash";
import moment from "moment";

export default {
  name: "ModeratorContestModerationPage",
  props: ['contest_id'],
  computed: {
    ...mapGetters(['currentUser', 'approvalStatuses']),
    ...mapGetters('moder_contests', [
      'currentModeratingContest',
      'currentModeratingContestLocalizer',
    ]),
    formatted_start_date() {
      return this.getFormattedFullDateTime(this.currentModeratingContest?.startDateTimeUTC)
    },
    sortedTasks() {
      return _.sortBy(this.currentModeratingContest.problems, ['letter'])
    },
    dataUrl() {
      if (!this.currentModeratingContest || this.currentModeratingContest.image) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.currentModeratingContest.image)) {
        const [file] = this.currentModeratingContest.image
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.currentModeratingContest.image
    },
  },
  methods: {
    ...mapActions('moder_contests', [
      'changeCurrentContest',
      'fetchContestsToModerate',
      'fetchRejectedContests',
      'fetchApprovedContests',
      'moderateContest',
    ]),
    ...mapActions(['deleteContest', 'getFormattedFullDateTime']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteContest(this.contest_id)
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async submitEntity() {
      this.error_msg = ''
      let {status, errors} = await this.moderateContest({
        contest_id: this.contest_id,
        request_body: {
          contestId: +this.contest_id,
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
      await this.fetchContestsToModerate(true)
      await this.fetchRejectedContests(true)
      await this.fetchDataAndGoToList(true)
      await this.changeCurrentContest({force: false, contest_id: null})
      await this.$router.push({
        name: 'ModeratorNotModeratedContestsPage'
      })
    }
  },
  data() {
    return {
      error_msg: '',
      message: '',
      current_status: null,
      schema: Yup.object({
        message: Yup.string(),
        current_status: Yup.number().required().nullable(),
      })
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      vm.message = vm.currentModeratingContest?.moderationMessage
      vm.current_status = +vm.currentModeratingContest?.approvalStatus
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