<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorNotModeratedProblemsPage",
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