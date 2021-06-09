<template>
  <!--eslint-disable -->
  <router-link :to="{name: 'WorkSpaceEditTaskPage'}" class="btn btn-primary text-light">Создать задачу</router-link>
  <task-preview-component v-for="task of availableTasks" :task="task"></task-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TaskPreviewComponent from "./TaskPreviewComponent";

export default {
  name: "MyTasksComponent",
  components: {TaskPreviewComponent},
  methods: {
    ...mapActions(['fetchAvailableTasks'])
  },
  computed: {
    ...mapGetters(['availableTasks'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAvailableTasks()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>