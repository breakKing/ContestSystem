<template>
    <router-link :to="{name: 'WorkSpaceEditContestPage'}" class="workspace-btn">Создать контест</router-link>
    <div class="row mt-3 gx-0">
        <contest-preview-component v-for="contest of currentUserContests" :contest="contest"></contest-preview-component>
    </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import ContestPreviewComponent from "../../contests/ContestPreviewComponent";

export default {
  name: "MyContestsComponent",
  components: {ContestPreviewComponent},
  methods: {},
  computed: {
    ...mapGetters(['currentUserContests'])
  }
}
</script>

<style lang="scss" scoped>

</style>