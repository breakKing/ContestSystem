<template>
  <v-form @submit="saveForm" :validation-schema="userSchema">
    <div v-if="!!success_message" class="alert alert-success" role="alert">
      {{ success_message }}
    </div>
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
      <label class="font-weight-bold">Ограничен в создании соревнований</label>
      <v-field v-model="isLimitedInContests" type="checkbox" class="form-control" name="isLimitedInContests"
               :value="true" :uncheckedValue="false"/>
      <error-message name="isLimitedInContests"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Ограничен в создании постов</label>
      <v-field v-model="isLimitedInPosts" type="checkbox" class="form-control" name="isLimitedInPosts" :value="true"
               :uncheckedValue="false"/>
      <error-message name="isLimitedInPosts"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Ограничен в создании курсов</label>
      <v-field v-model="isLimitedInCourses" type="checkbox" class="form-control" name="isLimitedInCourses" :value="true"
               :uncheckedValue="false"/>
      <error-message name="isLimitedInCourses"></error-message>
    </div>
    <div>
      <label class="font-weight-bold">Ограничен в создании задач</label>
      <v-field v-model="isLimitedInProblems" type="checkbox" class="form-control" name="isLimitedInProblems"
               :value="true" :uncheckedValue="false"/>
      <error-message name="isLimitedInProblems"></error-message>
    </div>
    <div>
      <div>
        <label class="font-weight-bold">Роли</label>
      </div>
      <div class="form-check d-flex align-items-center" v-for="role in all_roles">
        <label class="me-4">{{ role.description }}</label>
        <v-field v-model="roles" type="checkbox" :value="role.name" class="form-check-label" name="roles"/>
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
import 'moment-timezone'
import date_processing from "../mixins/date_processing";


export default {
  name: "UserFormComponent",
  mixins: [date_processing],
  props: {
    user_id: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      ...this.getInitParamsFromUser(),
      success_message: '',
      userSchema: Yup.object({
        firstName: Yup.string().required('не может быть пустым'),
        surname: Yup.string().required('не может быть пустым'),
        patronymic: Yup.string().nullable(),
        email: Yup.string().email().required('не может быть пустым'),
        phoneNumber: Yup.string().nullable(),
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
        dateOfBirth,
        isLimitedInContests,
        isLimitedInPosts,
        isLimitedInCourses,
        isLimitedInProblems,
        roles
      } = this.$store.getters.getUserById(this.user_id)
      return {
        firstName,
        surname,
        patronymic,
        email,
        phoneNumber,
        isLimitedInContests,
        isLimitedInPosts,
        isLimitedInCourses,
        isLimitedInProblems,
        dateOfBirth: this.to_local_string(dateOfBirth).substr(0, 10),
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
        dateOfBirth: this.dateOfBirth,
        isLimitedInContests: this.isLimitedInContests,
        isLimitedInPosts: this.isLimitedInPosts,
        isLimitedInCourses: this.isLimitedInCourses,
        isLimitedInProblems: this.isLimitedInProblems,
        roles: this.roles,
      }).then(async () => {
        await this.fetchAllUsers(true)
        this.success_message = 'Пользователь успешно обновлён'
      })
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAllRoles()
    })
  },
  async created() {
    moment.locale('ru')
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