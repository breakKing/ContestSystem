<template>
  <!--eslint-disable -->
  <div v-if="!!errors" class="alert alert-danger" role="alert">
    {{ errors }}
  </div>
  <v-form @submit="onSubmit" :validation-schema="schema" class="container-fluid mb-3">
    <div>
      <label class=" fs-3">Название</label>
      <v-field v-model="name" class="form-control" placeholder="Название программы-чекера" name="name"/>
      <error-message name="name"></error-message>
    </div>
    <div>
      <label class=" fs-3">Описание</label>
      <v-field v-model="description" class="form-control" name="description" as="textarea"
               placeholder="Краткое описание работы программы-чекера"/>
      <error-message name="description"></error-message>
    </div>
    <div>
      <v-field v-model="isPublic" class="custom-checkbox" id="isPublic" name="isPublic" type="checkbox" value="1"/>
      <label class=" fs-5" for="isPublic">Виден всем</label>
      <error-message name="isPublic"></error-message>
    </div>
    <div>
      <label class=" fs-3">Код</label>
      <v-field v-model="code" class="form-control" name="code" as="textarea" placeholder="Код программы-чекера"
               rows="6"/>
      <error-message name="code"></error-message>
    </div>
    <button type="submit">Сохранить</button>
  </v-form>
</template>

<script>
import {Field, Form, ErrorMessage} from "vee-validate";
import * as Yup from 'yup';
import {mapActions, mapGetters} from "vuex";
import axios from "axios";

export default {
  name: "CheckerEditComponent",
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  props: ['id'],
  data() {
    return {
      errors: '',
      name: '',
      description: '',
      code: '',
      isPublic: '',
      schema: Yup.object({
        name: Yup.string('Название должно быть строкой').required('Название это обязательное поле'),
        description: Yup.string('Описание должно быть строкой').required('Описание это обязательное поле'),
        code: Yup.string('Код должен быть строкой').required('Код это обязательное поле'),
      })
    }
  },
  computed: {
    ...mapGetters(['currentUser']),
  },
  methods: {
    ...mapActions(['getChecker', 'fetchCurrentUserCheckers', 'fetchAvailableCheckers']),
    async updateFields() {
      let checker = await this.getChecker(this.id)
      this.name = checker?.name
      this.description = checker?.description
      this.code = checker?.code
      this.isPublic = checker?.isPublic
    },
    async onSubmit() {
      if (!this.currentUser) {
        this.errors = 'Произошла ошибка авторизации'
        return
      }
      let request = {
        id: (!!this.id ? this.id : null),
        name: this.name,
        description: this.description,
        code: this.code,
        authorId: this.currentUser.id,
        isPublic: +this.isPublic === 1,
      }
      let result;
      try {
        if (this.id) {
          let {data} = await axios.put(`/api/checkers/edit-checker/${this.id}`, request)
          result = data
        } else {
          let {data} = await axios.post(`/api/checkers/add-checker`, request)
          result = data
        }
      } catch (e) {
        console.error(e)
        this.errors = 'При сохранении произошла фатальная ошибка'
      }
      if (result.status) {
        await this.fetchCurrentUserCheckers(true)
        await this.fetchAvailableCheckers(true)
        await this.$router.push({name: 'WorkSpaceMyApprovedCheckersPage'})
      } else if (result.errors) {
        this.errors = result.errors.join(', ')
      }
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.updateFields()
    })
  },
}
</script>

<style lang="scss" scoped>
    div * {
        margin: 5px;
        color: #04295E;
    }

span[role=alert] {
  color: red;
}

form {
  padding: 10px;
}

.custom-checkbox {
  position: absolute;
  z-index: -1;
  opacity: 0;
}

.custom-checkbox + label {
  display: inline-flex;
  align-items: center;
  user-select: none;
}

.custom-checkbox + label::before {
  content: '';
  display: inline-block;
  width: 1em;
  height: 1em;
  flex-shrink: 0;
  flex-grow: 0;
  border: 1px solid #adb5bd;
  border-radius: 0.25em;
  margin-right: 0.5em;
  background-repeat: no-repeat;
  background-position: center center;
  background-size: 50% 50%;
}

.custom-checkbox:checked + label::before {
  background-color: #0b76ef;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%23fff' d='M6.564.75l-3.59 3.612-1.538-1.55L0 4.26 2.974 7.25 8 2.193z'/%3e%3c/svg%3e");
}

.form-control {
  border-radius: 16px 0 16px 0;
}

button {
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
</style>