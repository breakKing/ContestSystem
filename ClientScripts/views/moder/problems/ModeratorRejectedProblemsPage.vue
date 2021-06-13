<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorRejectedProblemsPage",
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