<template>
  <div class="row">
    <div class="col">
      <div class="row">
        <div class="col-8">
          {{ contest.localizedName }}
        </div>
        <div class="col">
          {{ contest.creator?.fullName }}
        </div>
      </div>
      <div class="row">
        <div class="col">
          {{ contest.participatsCount || 0 }} участников
        </div>
      </div>
      <div class="row">
        <div class="col">
          {{ contest.localizedDescription }}
        </div>
      </div>
    </div>
    <div class="col" v-if="!!dataUrl">
      <img class="img-fluid" alt="картинка" :src="dataUrl"/>
    </div>
    <div class="col-2">
      <button v-if="currentUserIsOwner" type="button" class="btn btn-info" @click.prevent="editContest">Редактировать
      </button>
      <button type="button" class="btn btn-info" @click.prevent="goToContest">Войти
      </button>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ContestPreviewComponent",
  props: {
    contest: Object
  },
  methods: {
    async editContest() {
      await this.$router.push({name: 'WorkSpaceEditContestPage', params: {contest_id: this.contest.id}})
    },
    async goToContest() {
      await this.$router.push({name: 'ContestPage', params: {contest_id: this.contest.id}})
    },
  },
  computed: {
    ...mapGetters(['currentUser']),
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
      return 'data:image/jpeg;base64,' + this.contest.image

    },
  }
}
</script>

<style lang="scss" scoped>

</style>