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
  watch: {
    async $route(to, from) {
      await this.fetchRunningContests()
    }
  },
  async created() {
    await this.fetchAllUsers(true)
  },
}
</script>

<style lang="scss" scoped>

</style>