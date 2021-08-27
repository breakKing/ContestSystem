<template>
  <div class="row p-3">
    <div class="col">
      <h2 style="font-weight: bold;">{{ currentModeratingRuleSet && currentModeratingRuleSet.name }}</h2>
      <h4 style="color: #4998AB;">Автор: {{ currentModeratingRuleSet && currentModeratingRuleSet.author && currentModeratingRuleSet.author.fullName }}</h4>
      <br><p>Описание:</p>
      <p>{{ currentModeratingRuleSet && currentModeratingRuleSet.description }}</p><br>
      <p>Режим подсчёта: <span class="semi-bold-text">{{ readable_count_mode }}</span></p>
      <template v-if="currentModeratingRuleSet && +currentModeratingRuleSet.countMode === countModes.CountPenalty">
        <p v-if="currentModeratingRuleSet.penaltyForCompilationError"><span class="semi-bold-text">Наказывать</span> за ошибку компиляции</p>
        <p v-else><span class="semi-bold-text">Не наказывать</span> за ошибку компиляции</p>

        <p>Размер наказания за одну попытку: <span class="semi-bold-text">{{ currentModeratingRuleSet.penaltyForOneTry }}</span></p>
      </template>
      <template v-if="currentModeratingRuleSet && +currentModeratingRuleSet.countMode !== countModes.CountPoints">
        <p>Размер наказания за одну минуту: <span class="semi-bold-text">{{ currentModeratingRuleSet.penaltyForOneMinute }}</span></p>
      </template>
      <template v-if="currentModeratingRuleSet && +currentModeratingRuleSet.countMode === countModes.CountPoints">
        <p>Прибавка к очкам за лучшее решение: <span class="semi-bold-text">{{ currentModeratingRuleSet.pointsForBestSolution }}</span></p>
      </template>
      <p>Максимальное количество попыток на задачу: <span class="semi-bold-text">{{ currentModeratingRuleSet && currentModeratingRuleSet.maxTriesForOneProblem }}</span></p>
      <template v-if="currentModeratingRuleSet">
        <p v-if="currentModeratingRuleSet.publicMonitor"><span class="semi-bold-text">Публичный монитор</span></p>
        <p v-else><span class="semi-bold-text">Не публичный монитор</span></p>
      </template>
      <p>Замораживать монитор за <span class="semi-bold-text">{{ currentModeratingRuleSet && currentModeratingRuleSet.monitorFreezeTimeBeforeFinishInMinutes }}</span> минут до конца</p>

      <template v-if="currentModeratingRuleSet">
        <p v-if="currentModeratingRuleSet.showFullTestsResults"><span class="semi-bold-text">Показывать</span> полный отчёт о попытке</p>
        <p v-else><span class="semi-bold-text">Не показывать</span> полный отчёт о попытке</p>

        <p v-if="currentModeratingRuleSet.isPublic"><span class="semi-bold-text">Публичный</span> набор правил</p>
        <p v-else><span class="semi-bold-text">Не публичный</span> набор правил</p>
      </template><br>
      <div class="row">
        <div class="col">
          <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
            <div>
              <label>Комментарий</label>
              <v-field v-model="message" as="textarea" class="form-control" name="message"/>
              <error-message name="message"></error-message>
            </div>
            <div class="mt-3">
              <label>Статус</label>
              <v-field v-model="current_status" as="select" class="form-control" name="current_status">
                <option :value="approvalStatuses.NotModeratedYet">Не проверено</option>
                <option :value="approvalStatuses.Rejected">Отклонено</option>
                <option :value="approvalStatuses.Accepted">Утверждено</option>
              </v-field>
              <error-message name="current_status"></error-message>
            </div>
            <div class="mt-4">
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
      switch (+this.currentModeratingRuleSet?.countMode) {
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
  p {
    font-size: 1.2rem;
  }
  .semi-bold-text {
    font-weight: 600;
  }
</style>