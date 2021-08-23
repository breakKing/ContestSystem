<template>
  <checker-preview-component v-for="checker of checkersToModerate" :encode_html="true" :checker="checker"></checker-preview-component>
</template>

<script>
import CheckerPreviewComponent from "../../../components/user/workspace/checkers/CheckerPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "ModeratorNotModeratedCheckersPage",
  components: {CheckerPreviewComponent},
  computed: {
    ...mapGetters('moder_checkers', ['checkersToModerate'])
  },
  methods: {
    ...mapActions('moder_checkers', ['fetchCheckersToModerate'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchCheckersToModerate()
    })
  },
}
</script>

<style scoped>

</style>