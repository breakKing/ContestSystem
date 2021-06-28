<template>
    <div class="row mt-6">
        <contest-preview-component v-for="contest of currentUserApprovedContests" :contest="contest"></contest-preview-component>
    </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import ContestPreviewComponent from "../../contests/ContestPreviewComponent";

export default {
  name: "MyApprovedContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['currentUserApprovedContests'])
  }
}
</script>

<style lang="scss" scoped>

</style>