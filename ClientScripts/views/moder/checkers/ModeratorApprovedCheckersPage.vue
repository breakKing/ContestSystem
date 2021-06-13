<template>
  <checker-preview-component v-for="checker of approvedCheckers" :checker="checker"></checker-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import CheckerPreviewComponent from "../../../components/user/workspace/checkers/CheckerPreviewComponent";

export default {
  name: "ModeratorApprovedCheckersPage",
  components: {CheckerPreviewComponent},
  computed: {
    ...mapGetters('moder_checkers', ['approvedCheckers'])
  },
  methods: {
    ...mapActions('moder_checkers', ['fetchApprovedCheckers'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchApprovedCheckers()
    })
  },
}
</script>

<style scoped>

</style>