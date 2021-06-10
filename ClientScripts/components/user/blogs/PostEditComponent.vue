<template>
  <!--eslint-disable -->
  <button type="button" class="btn btn-primary ms-5 mt-2" data-bs-toggle="modal"
          :data-bs-target="'#'+modalId">
    {{ btnMessage }}
  </button>

  <teleport to="body">
    <div class="modal fade" :id="modalId" tabindex="-1" :aria-labelledby="modalId+'-title'"
         aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-xl">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" :id="modalId+'-title'">{{ modalMsg }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div v-if="!!error_msg" class="alert alert-danger" role="alert">
              {{ error_msg }}
            </div>
            <v-form @submit="savePost" :validation-schema="postCreationSchema">
              <div>
                <label>Изображение поста</label>
                <v-field v-model="previewImage" class="form-control" name="previewImage" type="file"/>
                <error-message name="previewImage"></error-message>
              </div>
              <div>
                <label>Название поста</label>
                <v-field v-model="postName" class="form-control" name="postName"/>
                <error-message name="postName"></error-message>
              </div>
              <div>
                <label>Текст поста</label>
                <ckeditor :editor="editor" v-model="postText" class="form-control" :config="editorConfig"
                          name="postText"></ckeditor>
                <error-message name="postText"></error-message>
              </div>
              <div>
                <label>Краткое описание</label>
                <v-field v-model="postPreview" class="form-control" name="postPreview" as="textarea"/>
                <error-message name="postPreview"></error-message>
              </div>
              <div class="form-group mt-2">
                <button class="btn btn-primary" type="submit">Сохранить</button>
              </div>
            </v-form>
          </div>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script>
import {Modal} from 'bootstrap';
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import {Field, Form, ErrorMessage} from "vee-validate";
import * as Yup from 'yup';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import $ from 'jquery';

export default {
  name: "PostEditComponent",
  props: ['post_id'],
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  data() {
    return {
      editor: ClassicEditor,
      editorData: '',
      editorConfig: {},
      previewImage: null,
      error_msg: '',
      postName: '',
      postText: '',
      postPreview: '',
      postCreationSchema: Yup.object({
        previewImage: Yup.mixed().nullable().required('Изображение поста это обязательное поле').label('Изображение поста'),
        postName: Yup.string().required('Название поста это обязательное поле').label('Название поста'),
        postText: Yup.string().required('Текст поста это обязательное поле').label('Текст поста'),
        postPreview: Yup.string().required('Превью поста это обязательное поле').label('Краткое описание'),
      })
    }
  },
  async mounted() {
    if (this.post_id) {
      let post_data = await this.getPostInfo(this.post_id)
      if (post_data) {
        this.postName = post_data.localizedName
        this.previewImage = post_data.previewImage
        this.postPreview = post_data.previewText
        this.postText = post_data.htmlLocalizedText
      }
    }
  },
  computed: {
    ...mapGetters(['currentUser']),
    modalId() {
      let name = (this.post_id || 'new')
      return `post-modal-${name}`
    },
    btnMessage() {
      if (!this.post_id) {
        return 'Создать статью'
      }
      return 'Редактировать статью'
    },
    modalMsg() {
      if (!this.post_id) {
        return 'Создать'
      }
      return 'Редактировать'
    }
  },
  methods: {
    ...mapActions(['getPostInfo', 'savePostInfo', 'fetchPostsList']),
    async savePost() {
      if (!this.currentUser) {
        this.error_msg = 'С вашими авторизационными данными что-то не так'
        return
      }
      let formData = new FormData();
      let tmp_form = $('<form enctype="multipart/form-data"></form>');
      tmp_form.append($('<input type="hidden"/>').attr('name', 'id').val(this.post_id))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'authorUserId').val(this.currentUser.id))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][culture]').val('ru'))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][name]').val(this.postName))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][htmlText]').val(this.postText))
      tmp_form.append($('<input type="hidden"/>').attr('name', 'localizers[0][previewText]').val(this.postPreview))

      tmp_form.append($('[name="previewImage"]').clone())

      let result = await this.savePostInfo({
        request_data: new FormData(tmp_form[0]),
        post_id: this.post_id
      })
      if (result.status) {
        await this.fetchPostsList(true);
        let modal = new Modal(document.querySelector('#' + this.modalId));
        modal.hide();
        this.error_msg = '';
      } else if (result.errors) {
        this.error_msg = result.errors.join(', ')
      }
    },
  },

}
</script>

<style lang="scss" scoped>

</style>