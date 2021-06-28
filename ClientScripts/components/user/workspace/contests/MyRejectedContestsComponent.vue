<template>
    <div class="row mt-6">
        <contest-preview-component v-for="contest of currentUserRejectedContests" :contest="contest"></contest-preview-component>
    </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import ContestPreviewComponent from "../../contests/ContestPreviewComponent";

export default {
  name: "MyRejectedContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['currentUserRejectedContests'])
  }
}
</script>

<style lang="scss" scoped>

</style>