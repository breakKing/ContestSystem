<template>
  <post-edit-component></post-edit-component>
  <h2 class="mt-5 text-center" v-if="!currentUserPostsList || currentUserPostsList.length === 0">Пока нет постов</h2>
  <template v-else>
    <post-preview-component v-for="post of currentUserPostsList" :post="post"
                            :edit-allowed="true"></post-preview-component>
  </template>
</template>

<script>
import PostEditComponent from "../../blogs/PostEditComponent";
import {mapActions, mapGetters} from "vuex";
import PostPreviewComponent from "../../blogs/PostPreviewComponent";

export default {
  name: "MyPostsComponent",
  components: {PostPreviewComponent, PostEditComponent},
  methods: {},
  computed: {
    ...mapGetters(['currentUserPostsList'])
  }
}
</script>

<style lang="scss" scoped>

</style>