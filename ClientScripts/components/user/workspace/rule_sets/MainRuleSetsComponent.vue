<template>
<router-view></router-view>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "MainRuleSetsComponent",
  methods: {
    ...mapActions(['fetchCurrentUserRuleSets'])
  },
  watch: {
    async $route(to, from) {
      await this.fetchCurrentUserRuleSets()
    }
  },
  async created() {
     await this.fetchCurrentUserRuleSets(true)
  },
}
</script>

<style scoped>

</style>