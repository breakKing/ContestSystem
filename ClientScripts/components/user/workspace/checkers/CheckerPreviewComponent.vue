<template>
  <div class="row">
    <div class="col-12 col-md-3">
      {{ checker.name }}
    </div>
    <div class="col-12 col-md-7">
      {{ checker.description }}
    </div>
    <div class="col-12 col-md">
      <button v-if="currentRole === 'user'" class="btn btn-primary"
              @click.prevent="$router.push({name: 'WorkSpaceEditCheckersPage', params: {id: +checker.id }})">
        Редактировать
      </button>
      <button v-else-if="currentRole === 'moderator'" class="btn btn-primary"
              @click.prevent="moderateChecker">
        Подробнее
      </button>
    </div>
  </div>
</template>

<script>
import {mapGetters} from "vuex";

export default {
  name: "CheckerPreviewComponent",
  props: {
    checker: Object
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole'])
  },
  methods: {
    async moderateChecker() {
      await this.$router.push({
        name: 'ModeratorCheckerModerationPage',
        params: {
          checker_id: +this.checker.id
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>

</style>