<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <contest-preview-component v-for="contest of runningContests" :contest="contest"></contest-preview-component>
</template>


<script>
import {mapActions, mapGetters} from 'vuex'
import ContestPreviewComponent from "./ContestPreviewComponent";
import CurrentlyRunningContestsBreads from "../../../dictionaries/bread_crumbs/CurrentlyRunningContestsBreads";
import BreadCrumbsComponent from "../../BreadCrumbsComponent";

export default {
  name: "CurrentlyRunningContestsComponent",
  components: {ContestPreviewComponent, BreadCrumbsComponent},
  computed: {
    ...mapGetters(['runningContests']),
    bread_crumb_routes() {
      return CurrentlyRunningContestsBreads
    }
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