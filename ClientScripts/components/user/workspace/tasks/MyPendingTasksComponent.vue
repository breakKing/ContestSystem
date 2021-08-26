<template>
  <div class="row gx-0 mt-3">
    <task-preview-component v-for="task of currentUserPendingTasks" :task="task"></task-preview-component>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TaskPreviewComponent from "./TaskPreviewComponent";

export default {
  name: "MyPendingTasksComponent",
  components: {TaskPreviewComponent},
  computed: {
    ...mapGetters(['currentUserPendingTasks'])
  }
}
</script>

<style lang="scss" scoped>

</style>