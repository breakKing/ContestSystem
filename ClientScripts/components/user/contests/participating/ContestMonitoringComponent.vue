<template>

</template>

<script>
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ContestMonitoringComponent",
  props: ['contest_id'],
  computed: {
    ...mapGetters(['currentUser', 'currentContest', 'currentContestMonitorEntries'])
  },
  methods: {
    ...mapActions(['changeCurrentContest'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: this.contest_id})
    })
  },
}
</script>

<style lang="scss" scoped>

</style>