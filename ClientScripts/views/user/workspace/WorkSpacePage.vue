<template>
  <div class="row ">
    <div class="col-3">
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