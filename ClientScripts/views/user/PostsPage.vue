<template>
  <!-- eslint-disable-next-line -->
  <posts-list-component></posts-list-component>
</template>

<script>
import PostsListComponent from "../../components/user/blogs/PostsListComponent";
import PostEditComponent from "../../components/user/blogs/PostEditComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "PostsPage",
  components: {PostEditComponent, PostsListComponent},
  computed: {
    ...mapGetters(['isAuthenticated'])
  },
  methods: {
    ...mapActions(['fetchPostsList']),
  },
  watch: {
    async $route(to, from) {
      if (to.name === 'PostsPage') {
        await this.fetchPostsList()
      }
    }
  },
  async created() {
    await this.fetchPostsList(true)
  },
}
</script>

<style lang="scss" scoped>

</style>