<template>
  <div class="row">
    <contest-preview-component v-for="contest of rejectedContests" :encode_html="true"
                               :contest="contest"></contest-preview-component>
  </div>
</template>

<script>
import ContestPreviewComponent from "../../../components/user/contests/ContestPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorRejectedContestsPage",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters('moder_contests', ['rejectedContests'])
  },
  methods: {
    ...mapActions('moder_contests', ['fetchRejectedContests'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRejectedContests()
    })
  },
}
</script>

<style scoped>

</style>