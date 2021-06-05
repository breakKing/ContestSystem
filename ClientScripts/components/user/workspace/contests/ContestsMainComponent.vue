<template>
  <router-view></router-view>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "ContestsMainComponent",
  methods: {
    ...mapActions(['fetchCurrentUserContestsList'])
  },
  watch: {
    async $route(to, from) {
      await this.fetchCurrentUserContestsList()
    }
  },
  async created() {
     await this.fetchCurrentUserContestsList(true)
  },
}
</script>

<style lang="scss" scoped>

</style>