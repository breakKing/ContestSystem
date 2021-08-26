<template>
  <div class="d-flex flex-column">
    <h4>Задачи</h4>
    <div class="card mb-2 custom-card" v-for="task of tasks" :class="calcClass(+task.id)">
      <div class="card-body custom-card-body">
        <router-link class="custom-card-item no_style_link"
                     active-class="active-item"
                     :to="{name: 'ContestParticipatingPage', params: {contest_id: currentContest?.id, task_id: task.problemId}}">
          {{ task.letter }} {{ task?.problem?.localizedName }}
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import {mapGetters} from "vuex";
import * as _ from 'lodash'
import TestResultVerdicts from "../../../../dictionaries/TestResultVerdicts";

export default {
  name: "TasksNavigationComponent",
  props: ['active_task_id', 'tasks', 'mapped_solutions'],
  computed: {
    ...mapGetters(['currentContest']),
  },
  methods: {
    calcClass(task_id) {
      let resolved = this.isResolved(+task_id)
      return {
        active: +task_id === +this.active_task_id,
        resolved: resolved,
        tried: !resolved && this.triedToResolve(+task_id)
      }
    },
    isResolved(task_id) {
      return _.some(this.mapped_solutions[task_id], (s) => +s.actualResult?.verdict === TestResultVerdicts.Accepted)
    },
    triedToResolve(task_id) {
      return (this.mapped_solutions[task_id] && this.mapped_solutions[task_id].length > 0)
    },

  }
}
</script>

<style lang="scss" scoped>

</style>