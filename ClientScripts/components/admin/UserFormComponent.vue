<template>
  <v-form @submit="saveForm" :validation-schema="userSchema">
    <div>
      <label class="font-weight-bold">Имя</label>
      <v-field v-model="firstName" class="form-control" name="firstName"/>
      <error-message name="firstName"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Фамилия</label>
      <v-field v-model="surname" class="form-control" name="surname"/>
      <error-message name="surname"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Отчество</label>
      <v-field v-model="patronymic" class="form-control" name="patronymic"/>
      <error-message name="patronymic"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Почта</label>
      <v-field v-model="email" type="email" class="form-control" name="email"/>
      <error-message name="email"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Телефон</label>
      <v-field v-model="phoneNumber" class="form-control" name="phoneNumber"/>
      <error-message name="phoneNumber"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Дата рождения</label>
      <v-field v-model="dateOfBirth" type="date" class="form-control" name="dateOfBirth"/>
      <error-message name="dateOfBirth"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Баланс</label>
      <v-field v-model="balance" type="number" class="form-control" name="balance"/>
      <error-message name="balance"></error-message>
    </div>
    <div>
      <div>
        <label class="font-weight-bold">Роли</label>
      </div>
      <div class="form-check d-flex align-items-center" v-for="role in all_roles">
        <label class="mr-4">{{ role.description }}</label>
        <v-field v-model="roles" type="checkbox" :value="role.id" class="form-check-label" name="roles"/>
      </div>
      <error-message name="roles"></error-message>
    </div>
    <div class="form-group mt-3">
      <button class="btn btn-primary" type="submit">Сохранить</button>
    </div>
  </v-form>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import {Field, Form, ErrorMessage} from "vee-validate"
import * as Yup from 'yup'
import * as _ from 'lodash'
import moment from 'moment'


export default {
  name: "UserFormComponent",
  props: {
    user_id: {
      type: String,
      required: true
    },
  },
  data() {
    return {
      ...this.getInitParamsFromUser(),
      userSchema: Yup.object({
        firstName: Yup.string().required('не может быть пустым'),
        surname: Yup.string().required('не может быть пустым'),
        patronymic: Yup.string().nullable(),
        email: Yup.string().email().required('не может быть пустым'),
        phoneNumber: Yup.string().nullable(),
        balance: Yup.number(),
        dateOfBirth: Yup.string().required('не может быть пустым'),
        roles: Yup.array().required('Выберите хотябы одну роль')
      })
    }
  },
  computed: {
    ...mapState({
      all_roles: state => state.auth.all_roles
    }),
    user() {
      return this.$store.getters.getUserById(this.user_id)
    }
  },
  methods: {
    ...mapActions(['fetchAllRoles', 'fetchAllUsers', 'updateUser']),
    getInitParamsFromUser() {
      let {
        firstName,
        surname,
        patronymic,
        email,
        phoneNumber,
        balance,
        dateOfBirth,
        roles
      } = this.$store.getters.getUserById(this.user_id)
      return {
        firstName,
        surname,
        patronymic,
        email,
        phoneNumber,
        balance,
        dateOfBirth: moment(dateOfBirth).toISOString().substr(0, 10),
        roles: _.map(roles, (r) => r.name),
      }
    },
    async saveForm() {
      await this.updateUser({
        id: this.user_id,
        firstName: this.firstName,
        surname: this.surname,
        patronymic: this.patronymic,
        email: this.email,
        phoneNumber: this.phoneNumber,
        balance: this.balance,
        dateOfBirth: this.dateOfBirth,
        roles: _.map(this.roles, (id) => {
          return {id}
        }),
      })
    }
  },
  watch: {
    async $route(to, from) {
      await this.fetchAllRoles()
    }
  },
  async created() {
    moment.locale('ru')
    await this.fetchAllRoles()
  },
  components: {
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
}
</script>

<style lang="scss" scoped>

</style>