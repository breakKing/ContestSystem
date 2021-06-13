<template>
  <div class="row">
    <div class="col">
      <h2>{{ currentModeratingChecker?.name }} {{ currentModeratingChecker?.author?.fullName }}</h2>
      <p>{{ currentModeratingChecker?.description }}</p>
      <textarea class="form-control code-input">{{currentModeratingChecker?.code}}</textarea>

      <div class="row">
        <div class="col">
          <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
            <div>
              <label>Комментарий</label>
              <v-field v-model="message" aas="textarea" class="form-control" name="message"/>
              <error-message name="message"></error-message>
            </div>
            <div>
              <label>Статус</label>
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
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";
import {mapActions, mapGetters} from "vuex";
import CodeMirror from "codemirror";

export default {
  name: "ModeratorCheckerModerationPage",
  props: ['checker_id'],
  computed: {
    ...mapGetters(['currentUser', 'approvalStatuses']),
    ...mapGetters('moder_checkers', [
      'currentModeratingChecker',
    ]),
  },
  methods: {
    ...mapActions('moder_checkers', [
      'changeCurrentChecker',
      'fetchCheckersToModerate',
      'fetchRejectedCheckers',
      'fetchApprovedCheckers',
      'moderateChecker',
    ]),
    ...mapActions(['deleteChecker']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteChecker(this.checker_id)
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async submitEntity() {
      this.error_msg = ''
      let {status, errors} = await this.moderateChecker({
        checker_id: this.checker_id,
        request_body: {
          checkerId: this.checker_id,
          approvalStatus: this.current_status,
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
      await this.fetchCheckersToModerate(true)
      await this.fetchRejectedCheckers(true)
      await this.fetchApprovedCheckers(true)
      await this.changeCurrentChecker({force: false, checker_id: null})
      await this.$router.push({
        name: 'ModeratorNotModeratedCheckersPage'
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
  mounted() {
    CodeMirror.fromTextArea(document.querySelector('.code-input'), {
      lineNumbers: true,
      readOnly: true,
    })
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentChecker({force: false, checker_id: vm.checker_id})
      vm.message = vm.currentModeratingChecker?.moderationMessage
      vm.current_status = +vm.currentModeratingChecker?.approvalStatus
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