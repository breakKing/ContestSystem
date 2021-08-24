<template>
  <table class="table">
    <thead>
    <tr>
      <th>Участник</th>
      <th>Действия</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="user of users">
      <td>{{ user.alias }}</td>
      <td>
        <a class="btn btn-danger" @click.prevent="kickUser(user)">Исключить</a>
      </td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from "vuex";

export default {
  name: "ContestUsersListComponent",
  props: {
    users: Array,
    contest: Object
  },
  methods: {
    ...mapActions(['removeUserFromContest', 'getContestParticipants', 'getChatsFromContest']),
    ...mapMutations(['setCurrentContestParticipants', 'setCurrentUserChats']),
    async kickUser(user) {
      let {status, errors} = await this.removeUserFromContest({
        user_id: user.id,
        contest_id: this.contest.id
      })
      if (status) {
        let participants = await this.getContestParticipants(this.contest.id)
        this.setCurrentContestParticipants(participants)

        let user_chats = _.filter(this.currentUserChats, (c) => +c.contestId !== +this.contest.id)
        let contest_chats = await this.getChatsFromContest({contest_id: this.contest.id})
        this.setCurrentUserChats(_.concat(user_chats, contest_chats))
      } else {
        console.error(errors)
      }
    }
  },
  computed: {
    ...mapGetters(['currentUserChats']),
  }
}
</script>

<style scoped>

</style>