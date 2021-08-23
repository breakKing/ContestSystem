<template>
  <contest-preview-component v-for="contest of contestsToModerate" :encode_html="true" :contest="contest"></contest-preview-component>
</template>

<script>
import ContestPreviewComponent from "../../../components/user/contests/ContestPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorNotModeratedContestsPage",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters('moder_contests', ['contestsToModerate'])
  },
  methods: {
    ...mapActions('moder_contests', ['fetchContestsToModerate'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchContestsToModerate()
    })
  },
}
</script>

<style scoped>

</style>