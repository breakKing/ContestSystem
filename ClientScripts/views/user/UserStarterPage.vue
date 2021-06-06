<template>
<!-- TODO контесты  -->
  <posts-list-component :max-posts="5"></posts-list-component>
</template>

<script>
import PostsListComponent from "../../components/user/blogs/PostsListComponent";
import {mapActions} from "vuex";
export default {
  name: "UserStarterPage",
  methods: {
    ...mapActions(['fetchPostsList']),
  },
  watch: {
    async $route(to, from) {
      if (to.name === 'UserStarterPage') {
        await this.fetchPostsList()
      }
    }
  },
  async created() {
    await this.fetchPostsList(true)
  },
  components: {PostsListComponent}
}
</script>

<style lang="scss" scoped>

</style>