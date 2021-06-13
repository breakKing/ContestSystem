<template>
  <post-preview-component v-for="post of postsToModerate" :edit-allowed="false" :post="post"></post-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import PostPreviewComponent from "../../../components/user/blogs/PostPreviewComponent";

export default {
  name: "ModeratorNotModeratedPostsPage",
  components: {PostPreviewComponent},
  computed: {
    ...mapGetters('moder_posts', ['postsToModerate'])
  },
  methods: {
    ...mapActions('moder_posts', ['fetchPostsToModerate'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchPostsToModerate()
    })
  },
}
</script>

<style scoped>

</style>