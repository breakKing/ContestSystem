<template>
  <header-component></header-component>
  <div class="mx-auto w-25" style="padding-top: 8%; height: 62.2vh">
    <div v-if="authError" class="alert alert-danger" role="alert">
      {{ authError }}
    </div>
    <h2>Вход в систему</h2>
    <v-form @submit="onSubmit" :validation-schema="loginSchema" class="mb-3">
      <div>
        <label>Логин</label>
        <v-field v-model="login" class="form-control" name="login"/>
        <error-message name="login"></error-message>
      </div>
      <div>
        <label>Пароль</label>
        <v-field v-model="password" class="form-control" name="password" type="password"/>
        <error-message name="password"></error-message>
      </div>
      <div class="form-group mt-2">
        <button class="btn btn-primary" type="submit">Войти</button>
      </div>
    </v-form>
    <router-link style="margin-top: 10px" :to="{name: 'Register'}">Ещё не зарегистрированы?</router-link>
  </div>
  <!-- eslint-disable-next-line -->
  <footer-component></footer-component>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import {Field, Form, ErrorMessage} from "vee-validate";
import * as Yup from 'yup';
import HeaderComponent from "../components/HeaderComponent";
import FooterComponent from "../components/FooterComponent";

export default {
  name: "LoginPage",
  props: {
    returnUrl: String,
  },
  components: {
    FooterComponent,
    HeaderComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  data() {
    return {
      login: '',
      password: '',
      loginSchema: Yup.object({
        login: Yup.string().required('Логин это обязательное поле').label('Логин'),
        password: Yup.string().required('Пароль это обязательное поле').label('Пароль'),
      })
    }
  },
  computed: {
    ...mapState({
      authError: state => state.auth.auth_error
    })
  },
  methods: {
    async onSubmit() {
      let result = await this.sendLoginRequest({
        username: this.login,
        password: this.password,
      })
      if (result) {
        if (this.returnUrl) {
          await this.$router.push({path: this.returnUrl})
        } else {
          await this.$router.push({name: 'Home'})
        }
      }
    },
    ...mapActions(['sendLoginRequest'])
  }
}
</script>

<style lang="scss" scoped>
span[role="alert"] {
  color: red;
}
</style>