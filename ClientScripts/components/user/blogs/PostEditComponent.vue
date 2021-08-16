<template>
  <!--eslint-disable -->
  <button type="button" class="workspace-btn mt-2 mb-1" data-bs-toggle="modal"
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
                <label class="fs-4">Изображение поста</label>
                <v-field v-model="previewImage" class="form-control" name="previewImage" type="file"/>
                <error-message name="previewImage"></error-message>
              </div>
              <div>
                <label class="fs-4">Название поста</label>
                <v-field v-model="postName" class="form-control" name="postName"/>
                <error-message name="postName"></error-message>
              </div>
              <div>
                <label class="fs-4">Текст поста</label>
                <v-field v-model="postText" name="postText" type="hidden"/>
                <quill-editor ref="quill_editor_postText" theme="snow" v-model:content="postText" contentType="html"
                              toolbar="full" class="form-control"></quill-editor>
                <error-message name="postText"></error-message>
              </div>
              <div>
                <label class="fs-4">Краткое описание</label>
                <v-field v-model="postPreview" class="form-control" name="postPreview" as="textarea"/>
                <error-message name="postPreview"></error-message>
              </div>
              <div class="form-group mt-2">
                <button type="submit">Сохранить</button>
              </div>
            </v-form>
          </div>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script>
import {mapActions, mapGetters} from 'vuex'
import {Field, Form, ErrorMessage} from "vee-validate";
import * as Yup from 'yup';
import $ from 'jquery';
import {QuillEditor} from "@vueup/vue-quill";

export default {
  name: "PostEditComponent",
  props: ['post_id'],
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
    QuillEditor
  },
  data() {
    return {
      previewImage: null,
      error_msg: '',
      postName: '',
      postText: '',
      postPreview: '',
      postCreationSchema: Yup.object({
        previewImage: Yup.mixed().nullable().required('Изображение поста это обязательное поле').label('Изображение поста'),
        postName: Yup.string('Название поста должно быть строкой').nullable().required('Название поста это обязательное поле').label('Название поста'),
        postText: Yup.string('Описание поста должно быть строкой').nullable().required('Описание поста это обязательное поле').label('Описание поста'),
        postPreview: Yup.string('Превью поста должно быть строкой').nullable().required('Превью поста это обязательное поле').label('Краткое описание'),
      })
    }
  },
  async mounted() {
    await this.updateFields();
  },
  computed: {
    ...mapGetters(['currentUser']),
    modalId() {
      let name = (this.post_id || 'new')
      return `post-modal-${name}`
    },
    btnMessage() {
      if (!this.post_id) {
        return 'Создать'
      }
      return 'Редактировать'
    },
    modalMsg() {
      if (!this.post_id) {
        return 'Создать'
      }
      return 'Редактировать'
    }
  },
  methods: {
    ...mapActions(['getLocalizedPost', 'savePostInfo', 'fetchPostsList', 'fetchUserWorkspacePostsList']),
    async updateFields() {
      if (this.post_id) {
        let post_data = await this.getLocalizedPost(this.post_id)
        this.postName = post_data.localizedName || null
        this.previewImage = post_data.previewImage || null
        this.postPreview = post_data.previewText || null
        this.postText = post_data.htmlLocalizedText || null
        this.$refs.quill_editor_postText.setHTML(this.postText)
      }
    },
    hideModal() {
      document.querySelector(`#${this.modalId} [data-bs-dismiss="modal"]`).click()
    },
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
        await this.fetchUserWorkspacePostsList(true);
        await this.updateFields()
        this.hideModal()
        this.error_msg = '';
      } else if (result.errors) {
        this.error_msg = result.errors.join(', ')
      }
    },
  },

}
</script>

<style lang="scss" scoped>
form div * {
  margin: 5px;
  color: #04295E;
}

span[role=alert] {
  color: red;
}

form {
  padding: 10px;
}

.form-control {
  border-radius: 16px;
}

button[type="submit"] {
  padding: 5px 10px;
  background-color: #fff;
  border-radius: 16px;
  border: 1px solid blue;

  &:hover {
    background-color: #0b76ef;
    color: white;
  }
}

.form-control::-webkit-input-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control::-moz-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control:-moz-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control:-ms-input-placeholder {
  opacity: 1;
  transition: opacity 0.3s ease;
}

.form-control:focus::-webkit-input-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.form-control:focus::-moz-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.form-control:focus:-moz-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.form-control:focus:-ms-input-placeholder {
  opacity: 0;
  transition: opacity 0.3s ease;
}

button[data-bs-target="#post-modal-new"] {
  padding: 5px 10px;
  width: fit-content;
}
</style>