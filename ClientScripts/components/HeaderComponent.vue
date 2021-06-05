<template>
<!--eslint-disable -->
<nav class="navbar navbar-expand-lg navbar-light bg-primary ">
    <div class="container-xl">
      <router-link class="navbar-brand me-5 text-light" :to="{name: 'Home'}">
        ContestSystem
        <!--<img src="~../../Images/logo.png" alt="logo">-->
      </router-link> 
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarsExample07XL" aria-controls="navbarsExample07XL" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarsExample07XL">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item me-2 fs-5">
            <router-link class="nav-link text-light" :to="{name: 'Home'}">Главная</router-link>
          </li>
          <li class="nav-item me-2 fs-5">
            <router-link class="nav-link text-light" :to="{name: 'Home'}">Контесты</router-link>
          </li>
          <li class="nav-item me-2 fs-5">
            <router-link class="nav-link text-light" :to="{name: 'PostsPage'}">Блог</router-link>
          </li>
          <template v-if="isAuthenticated && currentRole == 'user'">
          <li class="nav-item me-2 fs-5" >
            <router-link class="nav-link text-light" :to="{name: 'CoursePage'}">Учебные курсы</router-link>
          </li>
          <li class="nav-item fs-5">
            <router-link class="nav-link text-light" :to="{name: 'WorkSpaceWelcomeComponent'}">Личный кабинет</router-link>
          </li>
          </template>
        </ul>
        <ul>
          <template v-if="isAuthenticated ">
            <li class="nav-item me-2 fs-5 pt-3">
              <a class="nav-link text-light " href="#" @click.prevent="logout">Выйти</a>
            </li>
          </template>
          <template v-else>
            <li class="nav-item me-2 fs-5 mt-3">
              <router-link class="nav-link text-light" :to="{name: 'Login'}">Войти</router-link>
            </li>
            <li class="nav-item fs-5 mt-3">
              <router-link class="nav-link text-light" :to="{name: 'Register'}">Зарегистрироваться</router-link>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
export default {
  name: "HeaderComponent",
  computed: {
    ...mapGetters(['isAuthenticated', 'userName', 'currentRole'])
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
  .navbar-brand{
    font-family: cursive;
    padding: 15px;
    border-radius: 16px 0 16px 0;
    background-color: rgb(0, 13, 189, 0.2);
  }
  ul{
    list-style-type: none;
    display: inline-flex;
    li:hover,
    li:focus {
      border-radius: 16px;
      color: rgba(0, 0, 0, .85);
      background-color: rgba(210, 244, 234, 0.188);
}
  }
  .router-link-active {
    border-radius: 16px;
    background-color: #0A5BF0;
  }
</style>