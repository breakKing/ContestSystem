<template>
  <div class="row p-3">
    <div class="col">
      <h2>{{ currentModeratingRuleSet && currentModeratingRuleSet.name }} {{
          currentModeratingRuleSet && currentModeratingRuleSet.author && currentModeratingRuleSet.author.fullName
        }}</h2>
      <p>{{currentModeratingRuleSet && currentModeratingRuleSet.description}}</p>
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
            <button @click.prevent="deleteEntity" type="button" class="btn btn-danger">Удалить</button>
            <button type="submit" class="btn btn-primary ms-2">Сохранить</button>
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
  name: "ModeratorRuleSetModerationPage",
  props: ['rule_set_id'],
  mixins: [code_editor_mixin],
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
  computed: {
    ...mapGetters(['currentUser', 'approvalStatuses']),
    ...mapGetters('moder_rule_sets', [
      'currentModeratingRuleSet',
    ]),
  },
  methods: {
    ...mapActions('moder_rule_sets', [
      'changeCurrentRuleSet',
      'moderateRuleSet',
    ]),
    ...mapActions(['deleteRuleSet']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteRuleSet(this.rule_set_id)
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async submitEntity() {
      this.error_msg = ''
      let {status, errors} = await this.moderateRuleSet({
        rule_set_id: this.rule_set_id,
        request_body: {
          rulesSetId: +this.rule_set_id,
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
      await this.changeCurrentRuleSet({force: true, rule_set_id: null})
      await this.$router.push({
        name: 'ModeratorNotModeratedRuleSetsPage'
      })
    },
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentRuleSet({force: false, rule_set_id: vm.rule_set_id})
      vm.message = vm.currentModeratingRuleSet?.moderationMessage
      vm.current_status = +vm.currentModeratingRuleSet?.approvalStatus
      vm.error_msg = ''
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