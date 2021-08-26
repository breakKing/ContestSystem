<template>
    <div class="row gx-0 mt-3">
        <checker-preview-component v-for="checker of currentUserPendingCheckers" :checker="checker"></checker-preview-component>
    </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import CheckerPreviewComponent from "./CheckerPreviewComponent";

export default {
  name: "MyPendingCheckersComponent",
  components: {CheckerPreviewComponent},
  computed: {
    ...mapGetters(['currentUserPendingCheckers'])
  }
}
</script>

<style lang="scss" scoped>

</style>