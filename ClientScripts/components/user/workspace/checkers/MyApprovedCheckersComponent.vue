<template>
<!--eslint-disable -->
  <checker-preview-component v-for="checker of currentUserApprovedCheckers" :checker="checker">
  </checker-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import ContestPreviewComponent from "../../contests/ContestPreviewComponent";
import CheckerPreviewComponent from "./CheckerPreviewComponent";

export default {
  name: "MyApprovedCheckersComponent",
  components: {CheckerPreviewComponent},
  computed: {
    ...mapGetters(['currentUserApprovedCheckers'])
  }
}
</script>

<style lang="scss" scoped>

</style>