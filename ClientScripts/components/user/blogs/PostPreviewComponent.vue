<template>
  <div class="card mb-3">
    <div class="row g-0">
      <div class="col-md-7 col-12">
        <div class="card-body">
          <h5 class="card-title">{{ post.Name }}</h5>
          <p class="card-text">
            {{ post.previewText }}
          </p>
        </div>
        <post-edit-component v-if="editAllowed && currentUserIsAuthor"
                             :post_id="post.id"></post-edit-component>
        <button @click.prevent="previewClick" class="btn btn-info">Подробнее</button>
      </div>
      <div class="col-md-5 col-12">
        <img :src="dataUrl" :alt="post.Name">
      </div>
    </div>
  </div>
</template>

<script>
import PostEditComponent from "./PostEditComponent";
import {mapGetters} from "vuex";

export default {
  name: "PostPreviewComponent",
  components: {PostEditComponent},
  props: {
    post: Object,
    editAllowed: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    async previewClick() {
      await this.$router.push({
        name: 'ViewPost',
        params: {
          post_id: Number(this.post.id)
        }
      })
    }
  },
  computed: {
    ...mapGetters(['currentUser']),
    dataUrl() {
      if (!this.post) {
        return '';
      }
      return 'data:image/jpeg;base64,' + this.post?.previewImage

    },
    currentUserIsAuthor() {
      if (!this.currentUser) {
        return false
      }
      let result = false
      try {
        result = Number(this.post.author.id) === Number(this.currentUser.id)
      } catch (e) {
        console.error(e)
      }
      return result
    }
  },
}
</script>

<style lang="scss" scoped>
.card {
  cursor: pointer;
}
</style>