<template>
  <ul>
    <li v-for="task of tasks" :class="calcClass(+task.id)">
      <router-link :to="{name: 'ContestParticipatingPage', params: {contest_id: currentContest?.id, task_id: task.problemId}}">
        {{ task.letter }} {{ task?.problem?.localizedName }}
      </router-link>
    </li>
  </ul>
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
      return _.some(this.mapped_solutions[task_id], (s) => s.verdict === TestResultVerdicts.Accepted)
    },
    triedToResolve(task_id) {
      return (this.mapped_solutions[task_id] && this.mapped_solutions[task_id].length > 0)
    },

  }
}
</script>

<style lang="scss" scoped>

</style>