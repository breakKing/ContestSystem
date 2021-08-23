<template>
  <div class="col-md-4 mb-3">
    <div class="card">
      <div class="card-body d-flex flex-column justify-content-between">
        <h5 class="card-title">{{ checker.name }}</h5>
        <template v-if="checker && checker.description">
          <p class="card-text" v-if="encode_html">{{ checker.description }}</p>
          <p class="card-text" v-else v-html="checker.description"></p>
        </template>
        <p> Автор: {{ checker.author.fullName }}</p>
        <div class="row d-flex justify-content-between">
          <button v-if="currentRole === 'user' && currentUser && +currentUser.id === +checker.author.id"
                  class="workspace-btn me-2"
                  @click.prevent="$router.push({name: 'WorkSpaceEditCheckersPage', params: {id: +checker.id }})">
            Редактировать
          </button>
          <button v-else-if="currentRole === 'moderator'" class="workspace-btn"
                  @click.prevent="moderateChecker">
            Подробнее
          </button>
          <button v-if="currentUser && +currentUser.id === +checker.author.id" class="workspace-btn workspace-btn-del"
                  @click.prevent="deleteEntity">
            Удалить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import {mapGetters, mapActions} from "vuex";

export default {
  name: "CheckerPreviewComponent",
  props: {
    checker: Object,
    encode_html: {
      type: Boolean,
      default: false
    },
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole'])
  },
  methods: {
    ...mapActions(['deleteChecker',
      'fetchAvailableCheckers',
      'fetchCurrentUserCheckers']),
    async moderateChecker() {
      await this.$router.push({
        name: 'ModeratorCheckerModerationPage',
        params: {
          checker_id: +this.checker.id
        }
      })
    },
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteChecker(this.checker?.id)
      if (status) {
        await this.fetchData()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async fetchData() {
      await this.fetchAvailableCheckers(true)
      await this.fetchCurrentUserCheckers(true)
    }
  }
}
</script>

<style lang="scss" scoped>
.card {
  min-height: 230px;
  text-align: center;
  border: 1px solid blue;
}
</style>