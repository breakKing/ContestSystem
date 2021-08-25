<template>
  <header-component></header-component>
  <template v-if="$route.meta && $route.meta.no_gutters">
    <router-view></router-view>
  </template>
  <template v-else>
    <div class="row g-0">
      <div class="col side-vertical-line main-block-height">&nbsp;</div>
      <div class="col-9 px-md-4 my-md-3">
        <router-view></router-view>
      </div>
      <div class="col side-vertical-line main-block-height">&nbsp;</div>
    </div>
  </template>
  <footer-component></footer-component>
</template>

<script>
import HeaderComponent from "../../components/HeaderComponent";
import FooterComponent from "../../components/FooterComponent";

export default {
  name: "UserPage",
  components: {FooterComponent, HeaderComponent}
}
</script>

<style lang="scss" scoped>

</style>
