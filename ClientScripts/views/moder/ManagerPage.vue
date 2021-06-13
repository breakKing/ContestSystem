<template>
  <header-component></header-component>
  <div class="container">
    <div class="row">
      <div class="col-3" v-if="!$route.meta.hide_sidebar">
        <router-view name="sidebar"></router-view>
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

export default {
  name: "ManagerPage",
  components: {FooterComponent, HeaderComponent}
}
</script>

<style lang="scss" scoped>

</style>
