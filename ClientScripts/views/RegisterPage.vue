<template>
  <header-component></header-component>
  <div class="mx-auto w-25" style="margin: 5vh 0;">
    <div v-if="authError" class="alert alert-danger" role="alert">
      {{ authError }}
    </div>
    <h2>Регистрация</h2>
    <v-form @submit="onSubmit" :validation-schema="registrationSchema" class="mb-3">
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
      <div>
        <label>Повторите пароль</label>
        <v-field class="form-control" type="password" name="password_again"/>
        <error-message name="password_again"></error-message>
      </div>
      <div>
        <label>Почта</label>
        <v-field v-model="email" type="email" class="form-control" name="email"/>
        <error-message name="email"></error-message>
      </div>
      <div>
        <label>Телефон</label>
        <v-field v-model="phone" class="form-control" name="phone"/>
        <error-message name="phone"></error-message>
      </div>
      <div>
        <label>Имя</label>
        <v-field v-model="first_name" class="form-control" name="first_name"/>
        <error-message name="first_name"></error-message>
      </div>
      <div>
        <label>Фамилия</label>
        <v-field v-model="surname" class="form-control" name="surname"/>
        <error-message name="surname"></error-message>
      </div>
      <div>
        <label>Отчество</label>
        <v-field v-model="patronymic" class="form-control" name="patronymic"/>
        <error-message name="patronymic"></error-message>
      </div>
      <div>
        <label>Дата рождения</label>
        <v-field v-model="date_of_birth" type="date" class="form-control" name="date_of_birth"/>
        <error-message name="date_of_birth"></error-message>
      </div>
      <div class="form-group mt-3">
        <button class="btn btn-primary" type="submit">Зарегистрироваться</button>
      </div>
    </v-form>
    <router-link :to="{name: 'Login'}">Уже зарегистрировны?</router-link>
  </div>
  <footer-component></footer-component>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from 'yup';
import HeaderComponent from "../components/HeaderComponent";
import FooterComponent from "../components/FooterComponent";

export default {
  name: "RegisterPage",
  data() {
    return {
      login: '',
      password: '',
      phone: '',
      email: '',
      first_name: '',
      surname: '',
      patronymic: '',
      date_of_birth: null,
      registrationSchema: Yup.object({
        login: Yup.string().required('Логин это обязательное поле').label('Логин'),
        password: Yup.string().required('Пароль это обязательное поле').label('Пароль').min(5, 'Минимум 5 символов'),
        first_name: Yup.string().required('Имя это обязательное поле'),
        surname: Yup.string().required('Фамилия это обязательное поле'),
        patronymic: Yup.string().nullable(),
        phone: Yup.string().nullable(),
        email: Yup.string().email().required('Обязательное поле'),
        password_again: Yup.string().oneOf([Yup.ref('password'), null], 'Пароли не совпадают'),
        date_of_birth: Yup.date().required('Дата рождения это обязательное поле').nullable(),
      })
    }
  },
  components: {
    FooterComponent,
    HeaderComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  methods: {
    async onSubmit() {
      let result = await this.sendRegistrationRequest({
        UserName: this.login,
        Password: this.password,
        Email: this.email,
        Phone: this.phone,
        FirstName: this.first_name,
        Surname: this.surname,
        Patronymic: this.patronymic,
        DateOfBirth: this.date_of_birth,
        Fingerprint: this.fingerprint
      })
      if (result) {
        await this.$router.push({name: 'Home'})
      }
    },
    ...mapActions(
        ['sendRegistrationRequest']
    ),
  },
  computed: {
    ...mapState({
      authError: state => state.auth.auth_error
    }),
    ...mapGetters(["fingerprint"])
  },
}
</script>

<style lang="scss" scoped>
span[role="alert"] {
  color: red;
}
</style>