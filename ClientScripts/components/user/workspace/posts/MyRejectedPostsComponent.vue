<template>
  <div class="row gx-0 mt-3">
    <post-preview-component v-for="post of currentUserRejectedPostsList" :post="post"
                            :edit-allowed="true"></post-preview-component>
  </div>
</template>

<script>
import PostEditComponent from "../../blogs/PostEditComponent";
import {mapActions, mapGetters} from "vuex";
import PostPreviewComponent from "../../blogs/PostPreviewComponent";

export default {
  name: "MyRejectedPostsComponent",
  components: {PostPreviewComponent, PostEditComponent},
  computed: {
    ...mapGetters(['currentUserRejectedPostsList'])
  }
}
</script>

<style lang="scss" scoped>

</style>