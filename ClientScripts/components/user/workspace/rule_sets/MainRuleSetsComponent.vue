<template>
  <div class="p-3">
    <router-view></router-view>
  </div>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "MainRuleSetsComponent",
  methods: {
    ...mapActions(['fetchCurrentUserRuleSets'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchCurrentUserRuleSets()
    })
  },
}
</script>

<style scoped>

</style>