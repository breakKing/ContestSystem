<template>
  <!--eslint-disable -->
  <task-preview-component v-for="task of currentUserRejectedTasks" :task="task"></task-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TaskPreviewComponent from "./TaskPreviewComponent";

export default {
  name: "MyRejectedTasksComponent",
  components: {TaskPreviewComponent},
  computed: {
    ...mapGetters(['currentUserRejectedTasks'])
  }
}
</script>

<style lang="scss" scoped>

</style>