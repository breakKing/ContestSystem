<template>
  <div class="row">
    <div class="col">
      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <v-form @submit="saveRule" :validation-schema="schema">
        <div>
          <label class="font-weight-bold">Название</label>
          <v-field v-model="name" class="form-control" name="name"/>
          <error-message name="name"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Описание</label>
          <v-field v-model="description" class="form-control" as="textarea" name="description"/>
          <error-message name="description"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Режим подсчёта</label>
          <v-field v-model="countMode" class="form-control" as="select" name="countMode">
            <option value="1">Считать очки за тесты</option>
            <option value="2">Считать штраф</option>
            <option value="3">Считать разность между очками и штрафом</option>
          </v-field>
          <error-message name="countMode"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Наказывать за ошибку компиляции</label>
          <v-field v-model="penaltyForCompilationError" type="checkbox" value="1"
                   name="penaltyForCompilationError"/>
          <error-message name="penaltyForCompilationError"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Размер наказания за одну попытку</label>
          <v-field v-model="penaltyForOneTry" type="number" class="form-control"
                   name="penaltyForOneTry"/>
          <error-message name="penaltyForOneTry"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Размер наказания за одну минуту</label>
          <v-field v-model="penaltyForOneMinute" type="number" class="form-control"
                   name="penaltyForOneMinute"/>
          <error-message name="penaltyForOneMinute"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Прибавка к очкам за лучшее решение</label>
          <v-field v-model="pointsForBestSolution" type="checkbox" value="1"
                   name="pointsForBestSolution"/>
          <error-message name="pointsForBestSolution"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Максимальное количество попыток на задачу</label>
          <v-field v-model="maxTriesForOneProblem" type="number" class="form-control"
                   name="maxTriesForOneProblem"/>
          <error-message name="maxTriesForOneProblem"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Сделать монитор публичным</label>
          <v-field v-model="publicMonitor" type="checkbox" value="1"
                   name="publicMonitor"/>
          <error-message name="publicMonitor"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Замораживать монитора за ... минут до конца</label>
          <v-field v-model="monitorFreezeTimeBeforeFinishInMinutes" type="number" class="form-control"
                   name="monitorFreezeTimeBeforeFinishInMinutes"/>
          <error-message name="monitorFreezeTimeBeforeFinishInMinutes"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Показывать полный отчёт о попытке</label>
          <v-field v-model="showFullTestsResults" type="checkbox" value="1"
                   name="showFullTestsResults"/>
          <error-message name="showFullTestsResults"></error-message>
        </div>
        <div>
          <label class="font-weight-bold">Сделать публичным</label>
          <v-field v-model="isPublic" type="checkbox" value="1"
                   name="isPublic"/>
          <error-message name="isPublic"></error-message>
        </div>
        <button type="submit" class="btn btn-primary">Сохранить</button>
      </v-form>
    </div>
  </div>
</template>

<script>
import {Field, Form, ErrorMessage} from "vee-validate";
import * as Yup from 'yup';
import {mapActions, mapGetters} from "vuex";
import axios from 'axios'

export default {
  name: "RuleSetEditComponent",
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  props: ['set_id'],
  computed: {
    ...mapGetters(['currentUser'])
  },
  data() {
    return {
      error_msg: '',

      ruleSet: null,

      name: null,
      description: null,
      countMode: null,
      penaltyForCompilationError: null,
      penaltyForOneTry: null,
      penaltyForOneMinute: null,
      pointsForBestSolution: null,
      maxTriesForOneProblem: null,
      publicMonitor: null,
      monitorFreezeTimeBeforeFinishInMinutes: null,
      showFullTestsResults: null,
      isPublic: null,

      schema: Yup.object({
        name: Yup.string().required(),
        description: Yup.string().required(),
      })
    }
  },

  methods: {
    ...mapActions(['getRuleSet', 'fetchAvailableRuleSets']),
    async updateFields() {
      this.ruleSet = await this.getRuleSet(this.set_id)

      this.name = this.ruleSet?.name
      this.description = this.ruleSet?.description
      this.countMode = this.ruleSet?.countMode
      this.penaltyForCompilationError = this.ruleSet?.penaltyForCompilationError
      this.penaltyForOneTry = this.ruleSet?.penaltyForOneTry
      this.penaltyForOneMinute = this.ruleSet?.penaltyForOneMinute
      this.pointsForBestSolution = this.ruleSet?.pointsForBestSolution
      this.maxTriesForOneProblem = this.ruleSet?.maxTriesForOneProblem
      this.publicMonitor = this.ruleSet?.publicMonitor
      this.monitorFreezeTimeBeforeFinishInMinutes = this.ruleSet?.monitorFreezeTimeBeforeFinishInMinutes
      this.showFullTestsResults = this.ruleSet?.showFullTestsResults
      this.isPublic = this.ruleSet?.isPublic
    },
    async saveRule() {
      let url;
      let method;
      if (this.ruleSet) {
        url = `/api/rules/edit-rules/${this.ruleSet.id}`
        method = 'put'
      } else {
        url = `/api/rules/add-rules`
        method = 'post'
      }
      let request = {
        id: this.ruleSet?.id,
        name: this.name,
        description: this.description,
        isPublic: +this.isPublic === 1,
        authorId: this.currentUser.id,
        countMode: +this.countMode,
        penaltyForCompilationError: +this.penaltyForCompilationError === 1,
        penaltyForOneTry: this.penaltyForOneTry,
        penaltyForOneMinute: this.penaltyForOneMinute,
        pointsForBestSolution: +this.pointsForBestSolution === 1,
        maxTriesForOneProblem: this.maxTriesForOneProblem,
        publicMonitor: +this.publicMonitor === 1,
        monitorFreezeTimeBeforeFinishInMinutes: this.monitorFreezeTimeBeforeFinishInMinutes,
        showFullTestsResults: +this.showFullTestsResults === 1,
      }
      try {
        let {data} = await axios[method](url, request)
        if (data.status) {
          this.error_msg = ''
          await this.fetchAvailableRuleSets(true)
          await this.$router.push({name: 'WorkSpaceMyRuleSetsPage'})
        } else {
          this.error_msg = (data.errors || []).join(', ')
        }
      } catch (e) {
        console.error(e)
      }
    }
  },
  async created() {
    await this.updateFields()
  },
  watch: {
    async $route(to, from) {
      if (to.name === 'WorkSpaceEditRuleSetPage') {
        await this.updateFields()
      }
    }
  },
}
</script>

<style lang="scss" scoped>

</style>
