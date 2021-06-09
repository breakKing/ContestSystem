<template>
  <router-view></router-view>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "TasksMainComponent",
  methods: {
    ...mapActions(['fetchCurrentUserTasks'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchCurrentUserTasks()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>