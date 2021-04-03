<template>
  <div class="row">
    <div class="col">
      <table class="table">
        <thead>
        <tr>
          <th>ФИО</th>
          <th>Email</th>
          <th>Дата рождения</th>
          <th>Роли</th>
          <th></th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="user in currentPageItems">
          <td>{{ user.fullName }}</td>
          <td>{{ user.email }}</td>
          <td>{{ formatDate(user.dateOfBirth) }}</td>
          <td>{{ getRolesListString(user.roles) }}</td>
          <td>
            <user-change-component :user_id="user.id"></user-change-component>
          </td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
  <v-pagination
      v-model="currentPage"
      :pages="total_pages"
      :range-size="2"
      active-color="#DCEDFF"
  />
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import * as _ from 'lodash'
import moment from 'moment'
import VPagination from "@hennge/vue3-pagination";
import "@hennge/vue3-pagination/dist/vue3-pagination.css";
import UserChangeComponent from "./UserChangeComponent";

export default {
  name: "UsersListComponent",
  data() {
    return {
      page_size: 10,
    }
  },
  computed: {
    ...mapState({
      users: state => state.admin.all_users
    }),
    totalItems() {
      return this.users.length
    },
    total_pages() {
      return Math.ceil(this.totalItems / this.page_size);
    },
    currentPageItems() {
      let offset = (this.currentPage - 1) * this.page_size;
      return _.drop(this.users, offset).slice(0, this.page_size)
    },
    currentPage: {
      get() {
        return Number(this.$route.query.page) || 1
      },
      set(val) {
        this.$router.replace({name: this.$route.name, query: {page: val}})
      }
    },
  },
  methods: {
    ...mapActions(['fetchAllUsers']),
    getRolesListString(roles) {
      return _.join(_.map(roles, (role) => role.description), '\n')
    },
    formatDate(date) {
      return moment(date).format('MMMM DD YYYY')
    }
  },
  watch: {
    async $route(to, from) {
      await this.fetchAllUsers()
    }
  },
  async created() {
    moment.locale('ru')
    await this.fetchAllUsers(true)
  },
  components: {
    UserChangeComponent,
    VPagination
  }
}
</script>

<style lang="scss" scoped>

</style>