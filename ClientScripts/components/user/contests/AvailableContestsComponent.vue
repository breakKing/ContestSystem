<template>
  <!--eslint-disable -->
  <contest-preview-component v-for="contest of availableContests" :contest="contest"></contest-preview-component>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import * as _ from 'lodash'
import ContestPreviewComponent from "./ContestPreviewComponent";

export default {
  name: "AvailableContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['availableContests'])
  },
  methods: {
    ...mapActions(['fetchAvailableContests']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAvailableContests()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>