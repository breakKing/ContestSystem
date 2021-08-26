<template>
    <div class="row gx-0 mt-3">
        <task-preview-component v-for="task of currentUserApprovedTasks" :task="task"></task-preview-component>
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