<template>
    <div class="row mt-3">
        <task-preview-component v-for="task of currentUserPendingTasks" :task="task"></task-preview-component>
    </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TaskPreviewComponent from "./TaskPreviewComponent";

export default {
  name: "MyApprovedTasksComponent",
  components: {TaskPreviewComponent},
  computed: {
    ...mapGetters(['currentUserApprovedTasks'])
  }
}
</script>

<style lang="scss" scoped>

</style>