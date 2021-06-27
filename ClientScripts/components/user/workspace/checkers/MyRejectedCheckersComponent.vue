<template>
    <div class="row mt-4">
        <checker-preview-component v-for="checker of availableCheckers" :checker="checker"></checker-preview-component>
    </div>
</template>

<script>
import {mapGetters} from "vuex";
import CheckerPreviewComponent from "./CheckerPreviewComponent";

export default {
  name: "MyRejectedCheckersComponent",
  components: {CheckerPreviewComponent},
  computed: {
    ...mapGetters(['currentUserRejectedCheckers'])
  }
}
</script>

<style lang="scss" scoped>

</style>