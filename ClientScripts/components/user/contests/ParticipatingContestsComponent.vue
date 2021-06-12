<template>
  <!--eslint-disable -->
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <contest-preview-component v-for="contest of participatingContests" :contest="contest"></contest-preview-component>
</template>


<script>
import {mapActions, mapGetters} from 'vuex'
import ContestPreviewComponent from "./ContestPreviewComponent";
import ParticipatingContestsBreads from "../../../dictionaries/bread_crumbs/ParticipatingContestsBreads";
import BreadCrumbsComponent from "../../BreadCrumbsComponent";

export default {
  name: "ParticipatingContestsComponent",
  components: {ContestPreviewComponent, BreadCrumbsComponent},
  computed: {
    ...mapGetters(['participatingContests']),
    bread_crumb_routes() {
      return ParticipatingContestsBreads
    }
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