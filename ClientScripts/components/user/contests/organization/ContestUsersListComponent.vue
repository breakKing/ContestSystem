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
import {mapActions, mapMutations} from "vuex";

export default {
  name: "ContestUsersListComponent",
  props: {
    users: Array,
    contest: Object
  },
  methods: {
    ...mapActions(['removeUserFromContest', 'getContestParticipants']),
    ...mapMutations(['setCurrentContestParticipants']),
    async kickUser(user) {
      let {status, errors} = await this.removeUserFromContest({
        user_id: user.userId,
        contest_id: this.contest.id
      })
      if (status) {
        let participants = await this.getContestParticipants(this.contest.id)
        this.setCurrentContestParticipants(participants)
      } else {
        console.error(errors)
      }
    }
  }
}
</script>

<style scoped>

</style>