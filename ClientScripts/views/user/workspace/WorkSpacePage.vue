<template>
  <div class="d-flex flex-row">
    <work-space-side-bar-component></work-space-side-bar-component>
    <div class="col gx-0">
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