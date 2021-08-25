<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <posts-list-component></posts-list-component>
</template>

<script>
import PostsListComponent from "../../components/user/blogs/PostsListComponent";
import PostEditComponent from "../../components/user/blogs/PostEditComponent";
import {mapActions, mapGetters} from "vuex";
import BreadCrumbsComponent from "../../components/BreadCrumbsComponent";
import PostsListBreads from "../../dictionaries/bread_crumbs/PostsListBreads";

export default {
  name: "PostsPage",
  components: {BreadCrumbsComponent, PostEditComponent, PostsListComponent},
  computed: {
    ...mapGetters(['isAuthenticated']),
    bread_crumb_routes() {
      return PostsListBreads
    }
  },
  methods: {
    ...mapActions(['fetchPostsList']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchPostsList()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>