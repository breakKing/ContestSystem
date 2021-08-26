<template>
  <div class="row p-3">
    <div class="col">
      <h2>{{ currentModeratingChecker && currentModeratingChecker.name }} {{
          currentModeratingChecker && currentModeratingChecker.author && currentModeratingChecker.author.fullName
        }}</h2>
      <p>{{ currentModeratingChecker && currentModeratingChecker.description }}</p>
      <prism-editor v-model="checkerCode"
                    :highlight="highlighter"
                    :tabSize="4"
                    line-numbers
                    readonly
                    class="code-editor"/>
      <div class="row">
        <div class="col">
          <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
            <div>
              <label>Комментарий</label>
              <v-field v-model="message" as="textarea" class="form-control" name="message"/>
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
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";
import {mapActions, mapGetters} from "vuex";
import code_editor_mixin from '../../../components/mixins/code_editor_mixin';

export default {
  name: "ModeratorCheckerModerationPage",
  props: ['checker_id'],
  mixins: [code_editor_mixin],
  data() {
    return {
      checkerCode: '',
      error_msg: '',
      message: '',
      current_status: null,
      schema: Yup.object({
        message: Yup.string().nullable(),
        current_status: Yup.number().required().nullable(),
      })
    }
  },
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
          checkerId: +this.checker_id,
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
      await this.changeCurrentChecker({force: true, checker_id: null})
      await this.$router.push({
        name: 'ModeratorNotModeratedCheckersPage'
      })
    },
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentChecker({force: false, checker_id: vm.checker_id})
      vm.message = vm.currentModeratingChecker?.moderationMessage
      vm.current_status = +vm.currentModeratingChecker?.approvalStatus
      vm.error_msg = ''
      vm.checkerCode = vm.currentModeratingChecker?.code
    })
  },
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage
  },
}
</script>

<style scoped>

</style>