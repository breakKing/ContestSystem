<template>
  <!--eslint-disable -->
  <contest-preview-component v-for="contest of availableContests" :contest="contest"></contest-preview-component>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import * as _ from 'lodash'
import ContestPreviewComponent from "./ContestPreviewComponent";

export default {
  name: "AvailablePostsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['availableContests'])
  },
  methods: {
    ...mapActions(['fetchAvailableContests']),
  },
  watch: {
    async $route(to, from) {
      if (to.name === 'AvailableContestsPage') {
        await this.fetchAvailableContests(true)
      }
    }
  },
  async created() {
    await this.fetchAvailableContests(true)
  },
}
</script>

<style lang="scss" scoped>

</style>