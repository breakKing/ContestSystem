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
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchCurrentUserContestsList()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>