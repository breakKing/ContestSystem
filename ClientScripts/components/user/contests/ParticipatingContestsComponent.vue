<template>
  <div class="row">
    <contest-preview-component v-for="contest of participatingContests" :contest="contest"></contest-preview-component>
  </div>
</template>


<script>
import {mapActions, mapGetters} from 'vuex'
import ContestPreviewComponent from "./ContestPreviewComponent";

export default {
  name: "ParticipatingContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['participatingContests']),
  },
  methods: {
    ...mapActions(['fetchParticipatingContests']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchParticipatingContests()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>