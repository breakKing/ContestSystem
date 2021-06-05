<template>
  <!--eslint-disable -->
  <router-link :to="{name: 'WorkSpaceEditTaskPage'}" class="btn btn-primary text-light">Создать задачу</router-link>
  <task-preview-component v-for="task of availableTasks" :task="task"></task-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import ContestPreviewComponent from "../../contests/ContestPreviewComponent";
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
  watch: {
    async $route(to, from) {
      await this.fetchAvailableTasks()
    }
  },
  async created() {
    await this.fetchAvailableTasks(true)
  },
}
</script>

<style lang="scss" scoped>

</style>