﻿<template>
    <div class="row gx-0 mt-3">
        <contest-preview-component v-for="contest of currentUserPendingContests"
                                   :contest="contest"></contest-preview-component>
    </div>
</template>

<script>
import PostEditComponent from "../../blogs/PostEditComponent";
import {mapActions, mapGetters} from "vuex";
import PostPreviewComponent from "../../blogs/PostPreviewComponent";
import ContestPreviewComponent from "../../contests/ContestPreviewComponent";

export default {
  name: "MyPendingContestsComponent",
  components: {ContestPreviewComponent},
  computed: {
    ...mapGetters(['currentUserPendingContests'])
  }
}
</script>

<style lang="scss" scoped>

</style>