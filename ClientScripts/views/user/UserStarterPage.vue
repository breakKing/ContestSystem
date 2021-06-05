<template>
<!-- TODO контесты  -->
  <posts-list-component :max-posts="5"></posts-list-component>
</template>

<script>
import PostsListComponent from "../../components/user/blogs/PostsListComponent";
export default {
  name: "UserStarterPage",
  components: {PostsListComponent}
}
</script>

<style lang="scss" scoped>

</style>