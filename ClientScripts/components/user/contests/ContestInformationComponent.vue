<template>
  <div class="row">
    <div class="col">
      <div class="row">
        <div class="col"><h2>{{ currentContest?.localizedName }}</h2></div>
        <div class="col">{{ this.currentContest?.creator?.fullName }}</div>
      </div>
      <p>{{ currentContest?.localizedDescription }}</p>
    </div>
    <div class="col-4" v-if="!!dataUrl">
      <img alt="картинка" class="img-fluid" :src="dataUrl"/>
    </div>
  </div>

  <div class="row" v-if="!currentUserIsOwnerOfCurrentContest">
    <div class="col">
      <template v-if="!currentUserIsParticipantOfCurrentContest">
        <button v-if="!wants_participate">
          Хочу учавствовать
        </button>
        <template v-else>
          <v-form @submit="joinContest" :validation-schema="schema">
            <div>
              <label>Псевдоним</label>
              <v-field v-model="nickname" class="form-control" name="nickname"/>
              <error-message name="nickname"></error-message>
            </div>
            <button type="submit" class="btn btn-primary">Учавствовать</button>
          </v-form>
        </template>
      </template>
      <template v-else>

      </template>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import {ErrorMessage, Field, Form} from "vee-validate";
import * as Yup from "yup";

export default {
  name: "ContestInformationComponent",
  components: {
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
    ...mapGetters([
      'currentContest',
      'currentUser',
      'currentContestParticipants',
      'currentContestMonitorEntries',
      'currentUserIsOwnerOfCurrentContest',
      'currentUserIsParticipantOfCurrentContest',
    ]),
    dataUrl() {
      if (!this.currentContest || !this.currentContest?.image) {
        return '';
      }
      return 'data:image/jpeg;base64,' + this.currentContest.image
    },
  },
  methods: {
    ...mapActions(['addUserToContest', 'changeCurrentContest']),
    resetParticipateTry() {
      this.wants_participate = false
      this.nickname = ''
    },
    async joinContest() {
      await this.addUserToContest({
        user_name: this.nickname,
        user_id: this.currentUser.id,
        contest_id: this.currentContest.id,
      })
      await this.changeCurrentContest({force: true, contest_id: this.currentContest?.id})
      await this.$router.push({name: 'ParticipatingContestsPage'})
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