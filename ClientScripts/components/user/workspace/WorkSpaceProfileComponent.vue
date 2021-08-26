<template>
  <div class="container px-4 py-3">
    <h3>Профиль</h3>
    <hr>
    <div class="row gx-0">
      <div class="col">
        <!--      ИНФА О ПОЛЬЗОВАТЕЛЕ-->
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Фамилия:</div>
          <div class="col">{{ currentUser.surname }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Имя:</div>
          <div class="col">{{ currentUser.firstName }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Отчество:</div>
          <div class="col">{{ currentUser.patronimyc }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Дата рождения:</div>
          <div class="col">{{ formatDate(currentUser.dateOfBirth) }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Электронная почта:</div>
          <div class="col">{{ currentUser.email }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Телефон:</div>
          <div class="col">{{ currentUser.phoneNumber }}</div>
        </div>
      </div>
      <div class="col">
        <!--      СТАТИСТИКА-->
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Создано постов:</div>
          <div class="col">{{ currentUserPostsList.length }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Создано соревнований:</div>
          <div class="col">{{ currentUserContests.length }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Создано задач:</div>
          <div class="col">{{ currentUserTasks.length }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Создано чекеров:</div>
          <div class="col">{{ currentUserCheckers.length }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Создано наборов правил:</div>
          <div class="col">{{ currentUserRuleSets.length }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Число соревнований, в которых принимаю участие:</div>
          <div class="col">{{ participatingContests.length }}</div>
        </div>
        <div class="row gx-0 mb-2">
          <div class="col fs-5">Число соревнований, доступных для участия:</div>
          <div class="col">{{ availableContests.length }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "WorkSpaceProfileComponent",
  computed: {
    ...mapGetters([
      'currentUser',
      'participatingContests',
      'availableContests',
      'currentUserContests',
      'currentUserPostsList',
      'formatDate',
      'currentUserTasks',
      'currentUserCheckers',
      'currentUserRuleSets',
    ])
  },
  methods: {
    ...mapActions([
      'fetchParticipatingContests',
      'fetchUserWorkspacePostsList',
      'fetchCurrentUserContestsList',
      'fetchCurrentUserTasks',
      'fetchCurrentUserCheckers',
      'fetchCurrentUserRuleSets',
      'fetchAvailableContests',
    ]),
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.fetchParticipatingContests()
      vm.fetchCurrentUserContestsList()
      vm.fetchUserWorkspacePostsList()
      vm.fetchCurrentUserTasks()
      vm.fetchCurrentUserCheckers()
      vm.fetchCurrentUserRuleSets()
      vm.fetchAvailableContests()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>