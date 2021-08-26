<template>
  <div class="row">
    <contest-preview-component v-for="contest of runningContests" :contest="contest"></contest-preview-component>
  </div>
</template>


<script>
import {mapActions, mapGetters} from 'vuex'
import ContestPreviewComponent from "./ContestPreviewComponent";

export default {
  name: "CurrentlyRunningContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['runningContests']),
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