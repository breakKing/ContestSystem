<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <div class="row">
    <div class="col">
      <div class="row">
        <div class="col-5">
          <h2>{{ currentContest && currentContest.localizedName }}</h2>
          <hr>
        </div>
        <div class="col">{{ currentContest && currentContest.creator && currentContest.creator.fullName }}</div>
      </div>
      <p v-html="currentContest && currentContest.localizedDescription"></p>
      <div class="row my-3"
           v-if="currentUserIsOrganizerOfCurrentContest || currentUserIsParticipantOfCurrentContest || (currentContest && currentContest.rules && currentContest.rules.publicMonitor)">
        <div class="col">
          <router-link class="btn btn-info"
                       :to="{name: 'ContestMonitorPage', params: {contest_id: currentContest.id}}">
            Монитор
          </router-link>
        </div>
      </div>

    </div>
    <div class="col-4" v-if="!!dataUrl">
      <img alt="картинка" class="img-fluid" :src="dataUrl"/>
    </div>
  </div>

  <div class="row" v-if="!currentUserIsOrganizerOfCurrentContest">
    <div class="col">
      <template v-if="!currentUserIsParticipantOfCurrentContest">
        <button class="btn btn-success" v-if="!wants_participate" @click.prevent="wants_participate=true">
          Хочу участвовать
        </button>
        <template v-else>
          <v-form @submit="joinContest" :validation-schema="schema">
            <div>
              <label>Псевдоним</label>
              <v-field v-model="nickname" class="form-control" name="nickname"/>
              <error-message name="nickname"></error-message>
            </div>
            <button type="submit" class="btn btn-primary">Участвовать</button>
          </v-form>
        </template>
      </template>
      <template v-else>
        <router-link class="btn btn-info" v-if="currentContestIsRunning"
                     :to="{name: 'ContestParticipatingPage', params: {contest_id: currentContest?.id}}">Задачи
        </router-link>
        <span v-else-if="currentContestIsInTheFuture">Соревнование начнётся {{
            formatted_start_date
          }}</span>
        <span v-else>Соревнование окончено</span>

        <router-link v-if="!currentContestIsInTheFuture" class="btn btn-success"
                     :to="{name: 'ContestMySolutionsPage', params: {contest_id: currentContest?.id }}">Мои отправки
        </router-link>
        <button class="btn btn-danger" v-else @click.prevent="removeFromParticipants">Не учавствовать</button>
      </template>
    </div>
  </div>
  <div v-else class="row">
    <div class="col">
      <contest-organizer-interface-main-component
          :contest="currentContest"
      ></contest-organizer-interface-main-component>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from "vuex";
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";
import BreadCrumbsComponent from "../../BreadCrumbsComponent";
import ContestPageBreads from "../../../dictionaries/bread_crumbs/contest/ContestPageBreads";
import ContestOrganizerInterfaceMainComponent from "./organization/ContestOrganizerInterfaceMainComponent";

export default {
  name: "ContestInformationComponent",
  components: {
    ContestOrganizerInterfaceMainComponent,
    BreadCrumbsComponent,
    VForm: Form,
    VField: Field,
    ErrorMessage,
  },
  data() {
    return {
      wants_participate: false,
      nickname: '',
      schema: Yup.object({
        nickname: Yup.string().required(),
      })
    }
  },
  props: ['contest_id'],
  computed: {
    ...mapGetters(['getFormattedFullDateTime']),
    ...mapGetters([
      'currentContest',
      'currentUser',
      'currentContestParticipants',
      'currentContestMonitorEntries',
      'currentUserIsOrganizerOfCurrentContest',
      'currentUserIsParticipantOfCurrentContest',
      'currentContestIsRunning',
      'currentContestIsInPast',
      'currentContestIsInTheFuture',
    ]),
    dataUrl() {
      if (!this.currentContest || !this.currentContest?.image) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.currentContest.image)) {
        const [file] = this.currentContest.image
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.currentContest.image
    },
    bread_crumb_routes() {
      return ContestPageBreads(this.contest_id)
    },
    formatted_start_date() {
      return this.getFormattedFullDateTime(this.currentContest?.startDateTimeUTC)
    }
  },
  methods: {
    ...mapActions(['addUserToContest',
      'changeCurrentContest',
      'removeUserFromContest',
      'getContestParticipants',
      'fetchRunningContests',
      'fetchAvailableContests',
      'fetchParticipatingContests',
      'fetchCurrentUserContestsList']),
    ...mapMutations(['setCurrentContestParticipants']),
    resetParticipateTry() {
      this.wants_participate = false
      this.nickname = ''
    },
    async joinContest() {
      let {status, errors} = await this.addUserToContest({
        user_name: this.nickname,
        user_id: this.currentUser.id,
        contest_id: this.currentContest.id,
      })
      if (status) {
        await this.changeCurrentContest({force: true, contest_id: this.currentContest?.id})
        await this.fetchData()
        await this.$router.push({name: 'ParticipatingContestsPage'})
      }
    },
    async removeFromParticipants() {
      let {status, errors} = await this.removeUserFromContest({
        user_id: this.currentUser?.id,
        contest_id: this.currentContest?.id
      })
      if (status) {
        let participants = await this.getContestParticipants(this.contest_id)
        this.setCurrentContestParticipants(participants)
        this.resetParticipateTry()
        await this.fetchData()
      }
    },
    async fetchData() {
      await this.fetchRunningContests(true)
      await this.fetchAvailableContests(true)
      await this.fetchParticipatingContests(true)
      await this.fetchCurrentUserContestsList(true)
    }
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      vm.resetParticipateTry()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>