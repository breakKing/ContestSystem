<template>
  <router-view></router-view>
</template>

<script>
import {mapActions} from "vuex";

export default {
  name: "PostsMainComponent",
  methods: {
    ...mapActions(['fetchUserPostsList'])
  },
  watch: {
    async $route(to, from) {
      await this.fetchUserPostsList()
    }
  },
  async created() {
     await this.fetchUserPostsList(true)
  },
}
</script>

<style lang="scss" scoped>

</style>