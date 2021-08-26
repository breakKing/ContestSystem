<template>
  <div class="row gx-0 mt-3">
    <post-preview-component v-for="post of currentUserApprovedPostsList" :post="post"
                            :edit-allowed="true"></post-preview-component>
  </div>
</template>

<script>
import PostEditComponent from "../../blogs/PostEditComponent";
import {mapActions, mapGetters} from "vuex";
import PostPreviewComponent from "../../blogs/PostPreviewComponent";

export default {
  name: "MyApprovedPostsComponent",
  components: {PostPreviewComponent, PostEditComponent},
  computed: {
    ...mapGetters(['currentUserApprovedPostsList'])
  }
}
</script>

<style lang="scss" scoped>

</style>