<template>
  <div class="container">
    <div v-if="!!error_msg" class="alert alert-danger" role="alert">
      {{ error_msg }}
    </div>
    <div class="row">
      <div class="col-12 col-md-9">
        <h2>{{ currentModeratingPostLocalizer?.name }}</h2>
        <span>{{ currentModeratingPostApprovalStatusName }}</span>
      </div>
      <div class="col">
        <p>{{ currentModeratingPost?.author?.fullName }}
          {{ getFormattedFullDateTime(currentModeratingPost?.publicationDateTimeUTC) }}</p>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <img class="img-fluid" :src="dataUrl" :alt="currentModeratingPostLocalizer?.name">
      </div>
    </div>
    <div class="row">
      <div class="col" v-html="currentModeratingPostLocalizer?.htmlText"></div>
    </div>
    <div class="row">
      <div class="col">
        <v-form @submit="submitEntity" :validation-schema="schema" class="mb-3">
          <div>
            <label>Комментарий</label>
            <v-field v-model="message" as="textarea" class="form-control" name="message"/>
            <error-message name="message"></error-message>
          </div>
          <div>
            <label>Статус</label>
            <v-field v-model="current_status" as="select" class="form-control" name="current_status">
              <option :value="approvalStatuses.NotModeratedYet">Не проверено</option>
              <option :value="approvalStatuses.Rejected">Отклонено</option>
              <option :value="approvalStatuses.Accepted">Утверждено</option>
            </v-field>
            <error-message name="current_status"></error-message>
          </div>
          <button @click.prevent="deleteEntity" type="button" class="btn btn-danger">Удалить</button>
          <button type="submit" class="btn btn-primary">Сохранить</button>
        </v-form>
      </div>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";

export default {
  name: "ModeratorPostModerationPage",
  props: ['post_id'],
  computed: {
    ...mapGetters(['currentUser', 'approvalStatuses']),
    ...mapGetters(['getFormattedFullDateTime']),
    ...mapGetters('moder_posts', [
      'currentModeratingPost',
      'currentModeratingPostLocalizer',
      'currentModeratingPostApprovalStatusName',
    ]),
    dataUrl() {
      if (!this.currentModeratingPost || !this.currentModeratingPost?.previewImage) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.currentModeratingPost.previewImage)) {
        const [file] = this.currentModeratingPost.previewImage
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.currentModeratingPost.previewImage
    },
  },
  methods: {
    ...mapActions('moder_posts', [
      'changeCurrentPost',
      'fetchPostsToModerate',
      'fetchRejectedPosts',
      'fetchApprovedPosts',
      'moderatePost',
    ]),
    ...mapActions(['deletePost']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deletePost(this.post_id)
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async submitEntity() {
      this.error_msg = ''
      let {status, errors} = await this.moderatePost({
        post_id: this.post_id,
        request_body: {
          postId: +this.post_id,
          approvalStatus: +this.current_status,
          approvingModeratorId: this.currentUser.id,
          moderationMessage: this.message,
        }
      })
      if (status) {
        await this.fetchDataAndGoToList()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async fetchDataAndGoToList() {
      await this.fetchPostsToModerate(true)
      await this.fetchRejectedPosts(true)
      await this.fetchApprovedPosts(true)
      await this.changeCurrentPost({force: false, post_id: null})
      await this.$router.push({
        name: 'ModeratorNotModeratedPostsPage'
      })
    }
  },
  data() {
    return {
      error_msg: '',
      message: '',
      current_status: null,
      schema: Yup.object({
        message: Yup.string().nullable(),
        current_status: Yup.number().required().nullable(),
      })
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentPost({force: false, post_id: vm.post_id})
      vm.message = vm.currentModeratingPost?.moderationMessage
      vm.current_status = +vm.currentModeratingPost?.approvalStatus
      vm.error_msg = ''
    })
  },
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
}
</script>

<style scoped>

</style>