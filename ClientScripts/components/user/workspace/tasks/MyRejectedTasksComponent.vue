<template>
  <!--eslint-disable -->
    <div class="row mt-6">
        <task-preview-component v-for="task of currentUserPendingTasks" :task="task"></task-preview-component>
    </div>
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