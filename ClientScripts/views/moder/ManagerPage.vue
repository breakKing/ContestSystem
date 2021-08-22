<template>
  <header-component></header-component>
  <div class="container" style="min-height: inherit;">
    <div class="row" style="min-height: inherit;">
      <div class="col-lg-3 col-12" v-if="!$route.meta.hide_sidebar">
        <moder-side-bar-component></moder-side-bar-component>
      </div>
      <div class="col">
        <router-view></router-view>
      </div>
    </div>
  </div>
  <!-- eslint-disable-next-line -->
  <footer-component></footer-component>
</template>

<script>
import HeaderComponent from "../../components/HeaderComponent";
import FooterComponent from "../../components/FooterComponent";
import ModerSideBarComponent from "../../components/moder/ModerSideBarComponent";

export default {
  name: "ManagerPage",
  components: {ModerSideBarComponent, FooterComponent, HeaderComponent}
}
</script>

<style lang="scss" scoped>

</style>
