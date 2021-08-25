<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <ul class="nav nav-pills mb-3" role="tablist">
    <li class="nav-item" role="presentation">
      <router-link class="nav-link" :to="{name: 'ParticipatingContestsPage'}">Участвую</router-link>
    </li>
    <li class="nav-item" role="presentation">
      <router-link class="nav-link" :to="{name: 'AvailableContestsPage'}">Доступные мне</router-link>
    </li>
    <li class="nav-item" role="presentation">
      <router-link class="nav-link" :to="{name: 'CurrentlyRunningContestsComponentPage'}">Идут сейчас</router-link>
    </li>
  </ul>
  <div class="tab-content">
    <div class="tab-pane fade show active" role="tabpanel">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
import ParticipatingContestsBreads from "../../../dictionaries/bread_crumbs/ParticipatingContestsBreads";
import AvailableContestsBreads from "../../../dictionaries/bread_crumbs/AvailableContestsBreads";
import CurrentlyRunningContestsBreads from "../../../dictionaries/bread_crumbs/CurrentlyRunningContestsBreads";
import BreadCrumbsComponent from "../../../components/BreadCrumbsComponent";

export default {
  name: "ContestsPage",
  components: {BreadCrumbsComponent},
  computed: {
    bread_crumb_routes() {
      if (this.$route.name === 'CurrentlyRunningContestsComponentPage') {
        return CurrentlyRunningContestsBreads
      } else if (this.$route.name === 'AvailableContestsPage') {
        return AvailableContestsBreads
      } else if (this.$route.name === 'ParticipatingContestsPage') {
        return ParticipatingContestsBreads
      }
      return {};
    }
  }
}
</script>

<style lang="scss" scoped>
.tab-pane {
  border: 1px solid #505050;
  padding: 1.5rem;
  min-height: 67vh;
}

.router-link-active {
  border-bottom: 1px solid black !important;
  border-radius: 0 !important;
}
</style>