<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorApprovedCoursesPage",
  computed: {
    ...mapGetters('moder_courses', ['approvedCourses'])
  },
  methods: {
    ...mapActions('moder_courses', ['fetchApprovedCourses'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchApprovedCourses()
    })
  },
}
</script>

<style scoped>

</style>