<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorNotModeratedCoursesPage",
  computed: {
    ...mapGetters('moder_courses', ['coursesToModerate'])
  },
  methods: {
    ...mapActions('moder_courses', ['fetchCoursesToModerate'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchCoursesToModerate()
    })
  },
}
</script>

<style scoped>

</style>