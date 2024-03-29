﻿<template>
  <div class="col-6 col-md-3 mb-3 mx-3">
    <div class="card h-100">
      <div class="p-2">
          <img v-if="!!dataUrl" :src="dataUrl" class="card-img-top" alt="..." style="max-height: 15rem; object-fit: contain;">
      </div>
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
            <button type="button" class="workspace-btn workspace-btn-enter mb-3"
                    @click.prevent="goToContest">
              Войти
            </button>
            <div class="row d-flex justify-content-between">
              <button v-if="currentUserIsOrganizer" type="button" class="workspace-btn" @click.prevent="editContest">
                Редактировать
              </button>
              <button v-if="currentUserIsOrganizer" class="workspace-btn workspace-btn-del"
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
import * as _ from "lodash";

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
    currentUserIsOrganizer() {
      if (!this.currentUser) {
        return false
      }
      return !!_.find((this.contest?.organizers || []), (o) => +o.id === +this.currentUser.id)
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