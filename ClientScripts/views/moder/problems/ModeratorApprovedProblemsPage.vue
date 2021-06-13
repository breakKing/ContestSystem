<template>
  <task-preview-component v-for="problem of approvedProblems" :task="problem"></task-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TaskPreviewComponent from "../../../components/user/workspace/tasks/TaskPreviewComponent";

export default {
  name: "ModeratorApprovedProblemsPage",
  components: {TaskPreviewComponent},
  computed: {
    ...mapGetters('moder_problems', ['approvedProblems'])
  },
  methods: {
    ...mapActions('moder_problems', ['fetchApprovedProblems'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchApprovedProblems()
    })
  },
}
</script>

<style scoped>

</style>