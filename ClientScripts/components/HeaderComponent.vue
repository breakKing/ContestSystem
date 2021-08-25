<template>
  <nav class="navbar navbar-expand-lg navbar-light header-block">
    <div class="container-xl">
      <router-link class="navbar-brand me-5 text-light" :to="{name: 'Home'}">
        <img src="~../../Images/logo.png" alt="logo" class="me-2">
        <span>ContestSystem</span>
      </router-link>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarsExample07XL"
              aria-controls="navbarsExample07XL" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarsExample07XL">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item me-2 fs-5 rounded-item hoverable_item">
            <router-link class="nav-link text-light" :to="{name: 'Home'}">Главная</router-link>
          </li>
          <li class="nav-item me-2 fs-5 rounded-item hoverable_item" v-if="isAuthenticated && currentRole === 'user'">
            <router-link class="nav-link text-light" :to="{name: 'AvailableContestsPage'}">Контесты</router-link>
          </li>
          <li class="nav-item me-2 fs-5 rounded-item hoverable_item">
            <router-link class="nav-link text-light" :to="{name: 'PostsPage'}">Блог</router-link>
          </li>
          <template v-if="isAuthenticated && currentRole === 'user'">
            <!--          <li class="nav-item me-2 fs-5 rounded-item" >-->
            <!--            <router-link class="nav-link text-light" :to="{name: 'CoursePage'}">Учебные курсы</router-link>-->
            <!--          </li>-->
            <li class="nav-item fs-5 rounded-item hoverable_item">
              <router-link class="nav-link text-light" :to="{name: 'WorkSpaceWelcomeComponent'}">Личный кабинет
              </router-link>
            </li>
          </template>
        </ul>
        <ul>
          <template v-if="isAuthenticated">
            <li class="nav-item me-2 fs-5 mt-3">
                <span class="nav-link text-light">{{userName}}</span>
            </li>
            <li class="nav-item fs-5 mt-3 auth-item">
              <a class="nav-link text-light" href="#" @click.prevent="logout">Выйти</a>
            </li>
          </template>
          <template v-else>
            <li class="nav-item me-3 fs-5 mt-3 auth-item">
              <router-link class="nav-link text-light" :to="{name: 'Login'}">Войти</router-link>
            </li>
            <li class="nav-item fs-5 mt-3 auth-item">
              <router-link class="nav-link text-light" :to="{name: 'Register'}">Зарегистрироваться</router-link>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script>
import {mapState, mapActions, mapGetters} from 'vuex'

export default {
  name: "HeaderComponent",
  computed: {
    ...mapGetters(['isAuthenticated', 'userName', 'currentRole'])
  },
  methods: {
    ...mapActions({
      logoutAction: 'logout'
    }),
    async logout() {
      await this.logoutAction();
      await this.$router.push({name: 'Login'})
    }
  },
}
</script>

<style lang="scss" scoped>
.header-block {
  background-color: #2D3A4F;

  .navbar-brand {
    span {
      font-weight: 700;
      letter-spacing: 1pt;
      font-size: 120%;
    }
  }

  .auth-item {
    background-color: #59B7CE;

    .router-link-active, .router-link-exact-active, .router-link {
      background-color: #59B7CE;
      &:hover, &:focus {
        background-color: #3a7787;
      }
    }
      &:hover, &:focus {
        background-color: #3a7787;
      }
  }

  .router-link-active, .router-link-exact-active, .router-link {
    background-color: #2D3A4F;
    &:hover, &:focus {
      border-radius: 0px;
    }
  }
  
}

ul {
  list-style-type: none;
  display: inline-flex;

  li:hover,
  li:focus {
    &.rounded-item {
      border-radius: 16px;
    }
  }
}

</style>