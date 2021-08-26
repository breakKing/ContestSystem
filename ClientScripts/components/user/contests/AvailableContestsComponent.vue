<template>
  <div class="row">
    <contest-preview-component v-for="contest of availableContests" :contest="contest"></contest-preview-component>
  </div>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import ContestPreviewComponent from "./ContestPreviewComponent";
import AvailableContestsBreads from "../../../dictionaries/bread_crumbs/AvailableContestsBreads";

export default {
  name: "AvailableContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['availableContests']),
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