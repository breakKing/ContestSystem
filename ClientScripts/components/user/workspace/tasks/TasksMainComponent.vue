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
  watch: {
    async $route(to, from) {
      await this.fetchCurrentUserTasks(true)
    }
  },
  async created() {
     await this.fetchCurrentUserTasks(true)
  },
}
</script>

<style lang="scss" scoped>

</style>