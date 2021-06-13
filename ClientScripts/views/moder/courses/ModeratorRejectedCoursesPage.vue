<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorRejectedCoursesPage",
  computed: {
    ...mapGetters('moder_courses', ['rejectedCourses'])
  },
  methods: {
    ...mapActions('moder_courses', ['fetchRejectedCourses'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRejectedCourses()
    })
  },
}
</script>

<style scoped>

</style>