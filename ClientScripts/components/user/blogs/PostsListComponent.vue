<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <h3 class="text-center mt-5" v-if="!limitedLatestPosts || limitedLatestPosts.length === 0">Пока нет записей :)</h3>
  <template v-else>
    <post-preview-component v-for="post of limitedLatestPosts" :post="post"></post-preview-component>
  </template>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import PostPreviewComponent from "./PostPreviewComponent"
import * as _ from 'lodash'
import BreadCrumbsComponent from "../../BreadCrumbsComponent";
import PostsListBreads from "../../../dictionaries/bread_crumbs/PostsListBreads";

export default {
  name: "PostsListComponent",
  components: {PostPreviewComponent, BreadCrumbsComponent},
  props: {
    maxPosts: {
      type: Number,
      default: null
    },
  },
  computed: {
    ...mapGetters(['latestPosts']),
    limitedLatestPosts() {
      if (!this.maxPosts) {
        return this.latestPosts
      }
      return _.slice(this.latestPosts, 0, this.maxPosts)
    },
    bread_crumb_routes() {
      return PostsListBreads
    }
  },
}
</script>

<style lang="scss" scoped>

</style>