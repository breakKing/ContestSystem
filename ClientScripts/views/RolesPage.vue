<template>
  <div class="row">
    <div class="col-4 offset-4">
      <div class="roles">
        <template v-for="role of currentUserRoles">
          <div class="role-item" @click="setCurrentRole(role)">
            <b>{{ mappedRoles[role].Description }}</b>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'

export default {
  name: "RolesPage",
  computed: {
    ...mapGetters(['currentRole', 'mappedRoles', 'currentUserRoles'])
  },
  methods: {
    ...mapMutations(['setCurrentRole']),
    ...mapActions(['fetchAllRoles',]),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAllRoles()
    })
  },
}
</script>

<style lang="scss" scoped>
.roles {
  margin-top: 10vh;
}
</style>