<template>
  <ul>
    <li v-for="task of tasks" :class="{active: +task.id === +active_task_id}">
      <router-link :to="{name: 'ContestParticipatingPage', params: {contest_id: currentContest?.id, task_id: task.id}}">
        {{ task.letter }} {{ task?.problem?.localizedName }}
      </router-link>
    </li>
  </ul>
</template>

<script>
import {mapGetters} from "vuex";

export default {
  name: "TasksNavigationComponent",
  props: ['active_task_id', 'tasks'],
  computed: {
    ...mapGetters(['currentContest'])
  }
}
</script>

<style lang="scss" scoped>

</style>