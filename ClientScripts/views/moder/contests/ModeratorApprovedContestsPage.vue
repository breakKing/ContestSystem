<template>
  <contest-preview-component v-for="contest of approvedContests" :encode_html="true" :contest="contest"></contest-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import ContestPreviewComponent from "../../../components/user/contests/ContestPreviewComponent";

export default {
  name: "ModeratorApprovedContestsPage",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters('moder_contests', ['approvedContests'])
  },
  methods: {
    ...mapActions('moder_contests', ['fetchApprovedContests'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchApprovedContests()
    })
  },
}
</script>

<style scoped>

</style>