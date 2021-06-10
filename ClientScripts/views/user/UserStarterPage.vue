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
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchPostsList()
    })
  },
  components: {PostsListComponent}
}
</script>

<style lang="scss" scoped>

</style>