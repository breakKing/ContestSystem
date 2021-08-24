<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <contest-solutions-list-component
      :solutions="currentContestSolutionsForCurrentUser"
      :contest="currentContest"
      :organizer_mode="false"
  ></contest-solutions-list-component>
</template>

<script>
import ContestSolutionsListComponent from "./ContestSolutionsListComponent";
import {mapActions, mapGetters} from "vuex";
import ContestMySolutionsBreads from "../../../../dictionaries/bread_crumbs/contest/ContestMySolutionsBreads";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";

export default {
  name: "ContestMySolutionsListComponent",
  components: {ContestSolutionsListComponent, BreadCrumbsComponent},
  props: ['contest_id'],
  computed: {
    ...mapGetters([
      'currentUser',
      'currentContestSolutionsForCurrentUser',
      'currentContestIsInPast',
      'currentContest'
    ]),
    bread_crumb_routes() {
      return ContestMySolutionsBreads(this.contest_id)
    }
  },
  methods: {
    ...mapActions(['changeCurrentContest']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (vm.currentContest && vm.currentContestIsInPast) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.currentContest.id}})
      }
    })
  }
}
</script>

<style scoped>

</style>