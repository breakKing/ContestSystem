<template>
  <div class="row">
    <div class="col">
      <div v-if="!!error_msg" class="alert alert-danger" role="alert">
        {{ error_msg }}
      </div>
      <v-form @submit="saveRule" :validation-schema="schema">
        <div>
          <label class="fs-4">Название</label>
          <v-field v-model="name" class="form-control" name="name"/>
          <error-message name="name"></error-message>
        </div>
        <div>
          <label class="fs-4">Описание</label>
          <v-field v-model="description" name="description" type="hidden"/>
          <quill-editor ref="quill_editor_description" theme="snow" v-model:content="description" contentType="html"
                    toolbar="full" class="form-control"></quill-editor>
          <error-message name="description"></error-message>
        </div>
        <div>
          <label class="fs-4">Режим подсчёта</label>
          <v-field v-model="countMode" class="form-control" as="select" name="countMode">
            <option value="1">Считать очки за тесты</option>
            <option value="2">Считать штраф</option>
            <option value="3">Считать разность между очками и штрафом</option>
          </v-field>
          <error-message name="countMode"></error-message>
        </div>
        <div v-show="+countMode === countModes.CountPenalty">
          <v-field v-model="penaltyForCompilationError"
                   type="checkbox" class="custom-checkbox"
                   :value="true" :uncheckedValue="false"
                   id="penaltyForCompilationError"
                   :disabled="+countMode !== countModes.CountPenalty"
                   name="penaltyForCompilationError"/>
          <label class="fs-4" for="penaltyForCompilationError">Наказывать за ошибку компиляции</label>

          <error-message name="penaltyForCompilationError"></error-message>
        </div>
        <div v-show="+countMode === countModes.CountPenalty">
          <label class="fs-4">Размер наказания за одну попытку</label>
          <v-field v-model="penaltyForOneTry" type="number" class="form-control"
                   :disabled="+countMode !== countModes.CountPenalty"
                   name="penaltyForOneTry"/>
          <error-message name="penaltyForOneTry"></error-message>
        </div>
        <div v-show="+countMode !== countModes.CountPoints">
          <label class="fs-4">Размер наказания за одну минуту</label>
          <v-field v-model="penaltyForOneMinute" type="number" class="form-control"
                   :disabled="+countMode === countModes.CountPoints"
                   name="penaltyForOneMinute"/>
          <error-message name="penaltyForOneMinute"></error-message>
        </div>
        <div v-show="+countMode === countModes.CountPoints">
          <v-field v-model="pointsForBestSolution" id="pointsForBestSolution" class="custom-checkbox" type="checkbox"
                   :disabled="+countMode !== countModes.CountPoints"
                   :value="true" :uncheckedValue="false"
                   name="pointsForBestSolution"/>
          <label class="fs-4" for="pointsForBestSolution">Прибавка к очкам за лучшее решение</label>
          <error-message name="pointsForBestSolution"></error-message>
        </div>
        <div>
          <label class="fs-4">Максимальное количество попыток на задачу</label>
          <v-field v-model="maxTriesForOneProblem" type="number" class="form-control"
                   name="maxTriesForOneProblem"/>
          <error-message name="maxTriesForOneProblem"></error-message>
        </div>
        <div>

          <v-field v-model="publicMonitor" id="publicMonitor" class="custom-checkbox" type="checkbox"
                   :value="true" :uncheckedValue="false"
                   name="publicMonitor"/>
          <label class="fs-4" for="publicMonitor">Сделать монитор публичным</label>
          <error-message name="publicMonitor"></error-message>
        </div>
        <div>
          <label class="fs-4">Замораживать монитор за ... минут до конца</label>
          <v-field v-model="monitorFreezeTimeBeforeFinishInMinutes" type="number" class="form-control"
                   name="monitorFreezeTimeBeforeFinishInMinutes"/>
          <error-message name="monitorFreezeTimeBeforeFinishInMinutes"></error-message>
        </div>
        <div>
          <v-field v-model="showFullTestsResults" id="showFullTestsResults" class="custom-checkbox" type="checkbox"
                   :value="true" :uncheckedValue="false"
                   name="showFullTestsResults"/>
          <label class="fs-4" for="showFullTestsResults">Показывать полный отчёт о попытке</label>
          <error-message name="showFullTestsResults"></error-message>
        </div>
        <div>
          <v-field v-model="isPublic" id="isPublic" class="custom-checkbox" type="checkbox"
                   :value="true" :uncheckedValue="false"
                   name="isPublic"/>
          <label class="fs-4" for="isPublic">Сделать набор правил публичным</label>
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
import CountModes from "../../../../dictionaries/CountModes";
import $ from "jquery";
import {QuillEditor} from "@vueup/vue-quill";

export default {
  name: "RuleSetEditComponent",
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
    QuillEditor
  },
  props: ['set_id'],
  computed: {
    ...mapGetters(['currentUser']),
    countModes() {
      return CountModes
    }
  },
  data() {
    return {
      error_msg: '',

      ruleSet: null,
      name: null,
      description: null,
      countMode: null,
      penaltyForCompilationError: 0,
      penaltyForOneTry: 0,
      penaltyForOneMinute: 0,
      pointsForBestSolution: false,
      maxTriesForOneProblem: null,
      publicMonitor: null,
      monitorFreezeTimeBeforeFinishInMinutes: null,
      showFullTestsResults: null,
      isPublic: null,

      schema: Yup.object({
        name: Yup.string('Название должно быть строкой').required('Название это обязательное поле').nullable(),
        description: Yup.string('Описание должно быть строкой').required('Описание это обязательное поле').nullable(),
      })
    }
  },

  methods: {
    ...mapActions(['getRuleSet', 'fetchAvailableRuleSets', 'fetchCurrentUserRuleSets']),
    async updateFields() {
      this.ruleSet = await this.getRuleSet(this.set_id)
      this.name = this.ruleSet?.name || null
      this.description = this.ruleSet?.description || null
      // у компонента баг. Начальное значение не отрисовывается
      this.$refs.quill_editor_description.setHTML(this.description)
      this.countMode = this.ruleSet?.countMode || CountModes.CountPenalty
      this.penaltyForCompilationError = this.ruleSet?.penaltyForCompilationError || false
      this.penaltyForOneTry = this.ruleSet?.penaltyForOneTry || 0
      this.penaltyForOneMinute = this.ruleSet?.penaltyForOneMinute || 0
      this.pointsForBestSolution = this.ruleSet?.pointsForBestSolution || false
      this.maxTriesForOneProblem = this.ruleSet?.maxTriesForOneProblem || 999
      this.publicMonitor = this.ruleSet?.publicMonitor || false
      this.monitorFreezeTimeBeforeFinishInMinutes = this.ruleSet?.monitorFreezeTimeBeforeFinishInMinutes || 0
      this.showFullTestsResults = this.ruleSet?.showFullTestsResults || false
      this.isPublic = this.ruleSet?.isPublic || false
    },
    async saveRule() {
      let url;
      let method;
      if (this.ruleSet) {
        url = `/api/workspace/rules${this.ruleSet.id}`
        method = 'put'
      } else {
        url = `/api/workspace/rules`
        method = 'post'
      }
      let request = {
        id: this.ruleSet?.id,
        name: this.name,
        description: this.description,
        isPublic: this.isPublic,
        authorId: this.currentUser.id,
        countMode: +this.countMode,
        penaltyForCompilationError: this.penaltyForCompilationError,
        penaltyForOneTry: this.penaltyForOneTry,
        penaltyForOneMinute: this.penaltyForOneMinute,
        pointsForBestSolution: this.pointsForBestSolution,
        maxTriesForOneProblem: this.maxTriesForOneProblem,
        publicMonitor: this.publicMonitor,
        monitorFreezeTimeBeforeFinishInMinutes: this.monitorFreezeTimeBeforeFinishInMinutes,
        showFullTestsResults: this.showFullTestsResults,
      }
      try {
        let {data} = await axios[method](url, request)
        if (data.status) {
          this.error_msg = ''
          await this.fetchAvailableRuleSets(true)
          await this.fetchCurrentUserRuleSets(true)
          await this.$router.push({name: 'WorkSpaceMyRuleSetsPage'})
        } else {
          this.error_msg = (data.errors || []).join(', ')
        }
      } catch (e) {
        console.error(e)
      }
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
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
