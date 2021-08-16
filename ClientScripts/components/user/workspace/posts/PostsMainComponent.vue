<template>
  <router-view></router-view>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "PostsMainComponent",
  methods: {
    ...mapActions(['fetchUserWorkspacePostsList'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchUserWorkspacePostsList()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>