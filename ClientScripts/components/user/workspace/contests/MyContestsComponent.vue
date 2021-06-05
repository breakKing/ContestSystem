<template>
  <router-link :to="{name: 'WorkSpaceEditContestPage'}" class="btn btn-info">Создать контест</router-link>
  <contest-preview-component v-for="contest of currentUserContests" :contest="contest"></contest-preview-component>
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