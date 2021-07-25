<template>
  <div class="card mt-3 post-wrap">
    <div class="row g-0">
      <div class="col-md-7 col-12">
        <div class="card-body d-flex flex-column justify-content-between align-items-center">
          <h5 class="card-title m-0">{{ post.localizedName }}</h5>
          <p class="card-text m-0">
            {{ post.previewText }}
          </p>
          <p class="card-text">
            Автор: {{ post?.author?.fullName }}
          </p>
        </div>
        <div class="d-flex flex-column justify-content-center align-items-center">
          <post-edit-component v-if="editAllowed && currentUserIsAuthor"
                               :post_id="post.id"></post-edit-component>
          <button v-if="canBeOpenedForModerationOrReading" @click.prevent="previewClick" class="workspace-btn mb-1">
            Подробнее
          </button>
          <button v-if="currentUserIsAuthor && isInWorkspace" class="workspace-btn workspace-btn-del mb-1"
                  @click.prevent="deleteEntity">
            Удалить
          </button>
        </div>

      </div>
      <div class="col-md-5 col-12">
        <img class="img-fluid" :src="dataUrl" :alt="post.Name">
      </div>
    </div>
  </div>
</template>

<script>
import PostEditComponent from "./PostEditComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "PostPreviewComponent",
  components: {PostEditComponent},
  props: {
    post: Object,
    editAllowed: {
      type: Boolean,
      default: false
    },
    isInWorkspace: {
      type: Boolean,
      default: true
    }
  },
  methods: {
    ...mapActions(['deletePost',
      'fetchPostsList',
      'fetchUserPostsList']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deletePost(this.post?.id)
      if (status) {
        await this.fetchData()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async previewClick() {
      if (this.currentRole === 'moderator') {
        await this.$router.push({
          name: 'ModeratorPostModerationPage',
          params: {
            post_id: +this.post.id
          }
        })
      } else {
        await this.$router.push({
          name: 'ViewPost',
          params: {
            post_id: +this.post.id
          }
        })
      }
    },
    async fetchData() {
      await this.fetchPostsList(true)
      await this.fetchUserPostsList(true)
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole']),
    dataUrl() {
      if (!this.post || !this.post.previewImage) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.post.previewImage)) {
        const [file] = this.post.previewImage
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.post.previewImage
    },
    currentUserIsAuthor() {
      if (!this.currentUser) {
        return false
      }
      let result = false
      try {
        result = +this.post.author.id === +this.currentUser.id
      } catch (e) {
        console.error(e)
      }
      return result
    },
    canBeOpenedForModerationOrReading() {
      if (this.isInWorkspace) {
        return this.currentRole === 'moderator' || (this.post && +this.post.approvalStatus === 2)
      }
      return true
    }
  },
}
</script>

<style lang="scss" scoped>
.card {
  border: 1px solid blue;
  padding-top: 10px;
  padding-bottom: 10px;
}

.post-wrap {
  margin-bottom: 15px;
}
</style>