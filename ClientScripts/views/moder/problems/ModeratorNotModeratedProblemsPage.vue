<template>
    <task-preview-component v-for="problem of problemsToModerate" :task="problem"></task-preview-component>
</template>

<script>
import TaskPreviewComponent from "../../../components/user/workspace/tasks/TaskPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorNotModeratedProblemsPage",
  components: { TaskPreviewComponent },
  computed: {
    ...mapGetters('moder_problems', ['problemsToModerate'])
  },
  methods: {
    ...mapActions('moder_problems', ['fetchProblemsToModerate'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchProblemsToModerate()
    })
  },
}
</script>

<style scoped>

</style>