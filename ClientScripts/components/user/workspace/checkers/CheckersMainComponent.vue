<template>
  <router-view></router-view>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "CheckersMainComponent",
  methods: {
    ...mapActions(['fetchCurrentUserCheckers'])
  },
  watch: {
    async $route(to, from) {
      await this.fetchCurrentUserCheckers()
    }
  },
  async created() {
     await this.fetchCurrentUserCheckers(true)
  },
}
</script>

<style lang="scss" scoped>

</style>