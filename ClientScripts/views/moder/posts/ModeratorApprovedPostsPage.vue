<template>
  <post-preview-component v-for="post of approvedPosts" :edit-allowed="false" :post="post"></post-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import PostPreviewComponent from "../../../components/user/blogs/PostPreviewComponent";

export default {
  name: "ModeratorApprovedPostsPage",
  components: {PostPreviewComponent},
  computed: {
    ...mapGetters('moder_posts', ['approvedPosts'])
  },
  methods: {
    ...mapActions('moder_posts', ['fetchApprovedPosts'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchApprovedPosts()
    })
  },
}
</script>

<style scoped>

</style>