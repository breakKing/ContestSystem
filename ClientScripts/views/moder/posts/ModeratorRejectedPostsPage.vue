<template>
  <post-preview-component v-for="post of rejectedPosts" :edit-allowed="false" :post="post"></post-preview-component>
</template>

<script>
import PostPreviewComponent from "../../../components/user/blogs/PostPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorRejectedPostsPage",
  components: {PostPreviewComponent},
  computed: {
    ...mapGetters('moder_posts', ['rejectedPosts'])
  },
  methods: {
    ...mapActions('moder_posts', ['fetchRejectedPosts'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRejectedPosts()
    })
  },
}
</script>

<style scoped>

</style>