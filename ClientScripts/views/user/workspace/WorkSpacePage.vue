<template>
  <div class="row" style="min-height: inherit;">
    <div class="col-lg-3 col-12">
      <work-space-side-bar-component></work-space-side-bar-component>
    </div>
    <div class="col mt-3">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>
import WorkSpaceSideBarComponent from "../../../components/user/workspace/WorkSpaceSideBarComponent";

export default {
  name: "WorkSpacePage",
  components: {WorkSpaceSideBarComponent}
}
</script>

<style lang="scss" scoped>

</style>