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
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchCurrentUserCheckers()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>