<template>
  <ul>
    <li v-for="task of tasks" :class="{active: +task.id === +active_task_id}"
        :class="(isResolved(+task.id) ? 'resolved' : (triedToResolve(+task.id) ? 'tried' : ''))">
      <router-link :to="{name: 'ContestParticipatingPage', params: {contest_id: currentContest?.id, task_id: task.id}}">
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