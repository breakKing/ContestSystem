<template>
  <div class="row p-3">
    <div class="col">
      <h2>{{ currentModeratingRuleSet && currentModeratingRuleSet.name }} {{
          currentModeratingRuleSet && currentModeratingRuleSet.author && currentModeratingRuleSet.author.fullName
        }}</h2>
      <p>{{ currentModeratingRuleSet && currentModeratingRuleSet.description }}</p>
      <p>Режим подсчёта: {{ readable_count_mode }}</p>
      <template v-if="currentModeratingRuleSet && +currentModeratingRuleSet.countMode === countModes.CountPenalty">
        <p v-if="currentModeratingRuleSet.currentModeratingRuleSet">Наказывать за ошибку компиляции</p>
        <p v-else>Не наказывать за ошибку компиляции</p>

        <p>Размер наказания за одну попытку: {{ currentModeratingRuleSet.penaltyForOneTry }}</p>
      </template>
      <template v-if="currentModeratingRuleSet && +currentModeratingRuleSet.countMode !== countModes.CountPoints">
        <p>Размер наказания за одну минуту: {{ currentModeratingRuleSet.penaltyForOneMinute }}</p>
      </template>
      <template v-if="currentModeratingRuleSet && +currentModeratingRuleSet.countMode === countModes.CountPoints">
        <p>Прибавка к очкам за лучшее решение: {{ currentModeratingRuleSet.pointsForBestSolution }}</p>
      </template>
      <p>Максимальное количество попыток на задачу: {{ currentModeratingRuleSet.maxTriesForOneProblem }}</p>
      <template v-if="currentModeratingRuleSet">
        <p v-if="currentModeratingRuleSet.publicMonitor">Публичный монитор</p>
        <p v-else>Не публичный монитор</p>
      </template>
      <p>Замораживать монитор за {{ currentModeratingRuleSet.monitorFreezeTimeBeforeFinishInMinutes }} минут до конца</p>

      <template v-if="currentModeratingRuleSet">
        <p v-if="currentModeratingRuleSet.showFullTestsResults">Показывать полный отчёт о попытке</p>
        <p v-else>Не показывать полный отчёт о попытке</p>

        <p v-if="currentModeratingRuleSet.isPublic">Публичный набор правил</p>
        <p v-else>Не публичный набор правил</p>
      </template>
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
import CountModes from "../../../dictionaries/CountModes";

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
    countModes() {
      return CountModes
    },
    readable_count_mode() {
      switch (+this.currentModeratingRuleSet.countMode) {
        case CountModes.CountPenalty:
          return 'штраф'
        case CountModes.CountPoints:
          return 'очки за тесты'
        case CountModes.CountPointsMinusPenalty:
          return 'разность между очками и штрафом'
      }
      return ''
    },
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