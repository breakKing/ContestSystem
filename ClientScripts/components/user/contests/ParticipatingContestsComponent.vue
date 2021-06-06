<template>
  <!--eslint-disable -->

  <contest-preview-component v-for="contest of participatingContests" :contest="contest"></contest-preview-component>
</template>


<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import * as _ from 'lodash'
import ContestPreviewComponent from "./ContestPreviewComponent";

export default {
  name: "ParticipatingContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['participatingContests'])
  },
  methods: {
    ...mapActions(['fetchParticipatingContests']),
  },
  watch: {
    async $route(to, from) {
      if (to.name === 'ParticipatingContestsPage') {
        await this.fetchParticipatingContests()
      }
    }
  },
  async created() {
    await this.fetchParticipatingContests(true)
  },
}
</script>

<style lang="scss" scoped>

</style>