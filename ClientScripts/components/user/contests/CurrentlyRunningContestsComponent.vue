<template>
  <!--eslint-disable -->
  <contest-preview-component v-for="contest of runningContests" :contest="contest"></contest-preview-component>
</template>


<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import * as _ from 'lodash'
import ContestPreviewComponent from "./ContestPreviewComponent";

export default {
  name: "CurrentlyRunningContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['runningContests'])
  },
  methods: {
    ...mapActions(['fetchRunningContests']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRunningContests()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>