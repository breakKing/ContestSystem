<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <!--eslint-disable -->
  <contest-preview-component v-for="contest of availableContests" :contest="contest"></contest-preview-component>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import ContestPreviewComponent from "./ContestPreviewComponent";
import BreadCrumbsComponent from "../../BreadCrumbsComponent";
import AvailableContestsBreads from "../../../dictionaries/bread_crumbs/AvailableContestsBreads";

export default {
  name: "AvailableContestsComponent",
  components: {ContestPreviewComponent,BreadCrumbsComponent},
  computed: {
    ...mapGetters(['availableContests']),
    bread_crumb_routes() {
      return AvailableContestsBreads
    }
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