<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ContestMonitoringComponent",
  props: ['contest_id'],
  computed: {
    ...mapGetters([
      'currentUser',
      'currentContest',
      'currentContestMonitorEntries',
      'currentUserIsOwnerOfCurrentContest',
      'currentUserIsParticipantOfCurrentContest'
    ])
  },
  methods: {
    ...mapActions(['changeCurrentContest'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (!(vm.currentUserIsOwnerOfCurrentContest || vm.currentUserIsParticipantOfCurrentContest || vm.currentContest?.rules?.publicMonitor)) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.contest_id}})
      }
    })
  },
}
</script>

<style lang="scss" scoped>

</style>