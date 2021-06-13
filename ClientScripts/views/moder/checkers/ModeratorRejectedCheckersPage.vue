<template>
  <checker-preview-component v-for="checker of rejectedCheckers" :checker="checker"></checker-preview-component>
</template>

<script>
import CheckerPreviewComponent from "../../../components/user/workspace/checkers/CheckerPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorRejectedCheckersPage",
  components: {CheckerPreviewComponent},
  computed: {
    ...mapGetters('moder_checkers', ['rejectedCheckers'])
  },
  methods: {
    ...mapActions('moder_checkers', ['fetchRejectedCheckers'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRejectedCheckers()
    })
  },
}
</script>

<style scoped>

</style>