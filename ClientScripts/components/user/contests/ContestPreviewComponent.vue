<template>
  <div class="col-md-6 mb-3">
    <div class="card">
      <img v-if="!!dataUrl" :src="dataUrl" class="card-img-top" alt="...">
      <div class="card-body d-flex flex-column justify-content-between">
        <h5 class="card-title">{{ contest && contest.localizedName }}</h5>
        <template v-if="contest && contest.localizedDescription">
          <p v-if="encode_html">{{ contest.localizedDescription }}</p>
          <p v-html="contest.localizedDescription" v-else></p>
        </template>
        <p> Участников: {{ (contest && contest.participantsCount) || 0 }}</p>
        <p> Автор: {{ contest && contest.creator && contest.creator.fullName }}</p>
        <div class="row d-flex justify-content-center">
          <template v-if="currentRole === 'user'">
            <button v-if="!currentUserIsOwner" type="button" class="workspace-btn workspace-btn-enter mb-3"
                    @click.prevent="goToContest">
              Войти
            </button>
            <div class="row d-flex justify-content-between">
              <button v-if="currentUserIsOwner" type="button" class="workspace-btn" @click.prevent="editContest">
                Редактировать
              </button>
              <button v-if="currentUserIsOwner" class="workspace-btn workspace-btn-del"
                      @click.prevent="deleteEntity">
                Удалить
              </button>
            </div>
          </template>
          <template v-else-if="currentRole === 'moderator'">
            <button @click.prevent="moderateContest" class="workspace-btn">Подробнее</button>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ContestPreviewComponent",
  props: {
    contest: Object,
    encode_html: {
      type: Boolean,
      default: false
    },
  },
  methods: {
    ...mapActions(['deleteContest',
      'fetchCurrentUserContestsList',
      'fetchAvailableContests',
      'fetchParticipatingContests',
      'fetchRunningContests']),
    async editContest() {
      await this.$router.push({name: 'WorkSpaceEditContestPage', params: {contest_id: this.contest.id}})
    },
    async goToContest() {
      await this.$router.push({name: 'ContestPage', params: {contest_id: this.contest.id}})
    },
    async moderateContest() {
      await this.$router.push({
        name: 'ModeratorContestModerationPage',
        params: {
          contest_id: +this.contest.id
        }
      })
    },
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteContest(this.contest?.id)
      if (status) {
        await this.fetchData()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async fetchData() {
      await this.fetchCurrentUserContestsList(true)
      await this.fetchAvailableContests(true)
      await this.fetchParticipatingContests(true)
      await this.fetchRunningContests(true)
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole']),
    currentUserIsOwner() {
      if (!this.currentUser || !this.contest?.creator) {
        return false
      }
      return +this.contest.creator.id === +this.currentUser.id
    },
    dataUrl() {
      if (!this.contest || !this.contest?.image) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.contest.image)) {
        const [file] = this.contest.image
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.contest.image

    },
  }
}
</script>

<style lang="scss" scoped>
.card {
  text-align: center;
  border: 1px solid blue;
}

</style>