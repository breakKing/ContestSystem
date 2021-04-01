<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light mx-5 py-3 px-5 mb-4">
    <router-link class="navbar-brand" :to="{name: 'Home'}">{{ userName || 'Питомник' }}</router-link>
    <div class="collapse navbar-collapse">
      <ul class="navbar-nav ml-auto">
        <template v-if="isAuthenticated">
          <li class="nav-item">
            <a class="nav-link" href="#" @click.prevent="logout">Выйти</a>
          </li>
        </template>
        <template v-else>
          <li class="nav-item">
            <router-link class="nav-link" :to="{name: 'Login'}">Войти</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" :to="{name: 'Register'}">Зарегистрироватся</router-link>
          </li>
        </template>
      </ul>
    </div>
  </nav>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'

export default {
  name: "HeaderComponent",
  computed: {
    ...mapGetters(['isAuthenticated', 'userName'])
  },
  methods: {
    ...mapActions({
      logoutAction: 'logout'
    }),
    async logout(){
      await this.logoutAction();
      await this.$router.push({name: 'Login'})
    }
  },
}
</script>

<style lang="scss" scoped>

</style>