<template>
  <h3 class="text-center mt-5" v-if="!limitedLatestPosts || limitedLatestPosts.length===0">Пока нет записей :)</h3>
  <template v-else>
    <post-preview-component v-for="post of limitedLatestPosts" :post="post"></post-preview-component>
  </template>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import PostPreviewComponent from "./PostPreviewComponent"
import * as _ from 'lodash'

export default {
  name: "PostsListComponent",
  components: {PostPreviewComponent},
  props: {
    maxPosts: {
      type: Number,
      default: null
    },
  },
  computed: {
    ...mapGetters(['postsList', 'latestPosts']),
    limitedLatestPosts() {
      if (!this.maxPosts) {
        return this.latestPosts
      }
      return _.slice(this.latestPosts, 0, this.maxPosts)
    }
  },
  methods: {
    ...mapActions(['fetchPostsList', 'getPostInfo']),
  },
  watch: {
    async $route(to, from) {
      await this.fetchPostsList()
    }
  },
  async created() {
    await this.fetchPostsList(true)
  },
}
</script>

<style lang="scss" scoped>

</style>