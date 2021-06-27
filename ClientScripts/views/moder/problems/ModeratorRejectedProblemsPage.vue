<template>
    <task-preview-component v-for="problem of rejectedProblems" :task="problem"></task-preview-component>
</template>

<script>
import TaskPreviewComponent from "../../../components/user/workspace/tasks/TaskPreviewComponent";
import {mapActions, mapGetters} from "vuex";


export default {
  name: "ModeratorRejectedProblemsPage",
  components: { TaskPreviewComponent },
  computed: {
    ...mapGetters('moder_problems', ['rejectedProblems'])
  },
  methods: {
    ...mapActions('moder_problems', ['fetchRejectedProblems'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRejectedProblems()
    })
  },
}
</script>

<style scoped>

</style>