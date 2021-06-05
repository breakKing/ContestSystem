<template>
  <header-component></header-component>
  <div class="container" style="min-height: 62.2vh">
    <div class="row">
      <div class="col-4">
        <admin-side-bar-component></admin-side-bar-component>
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
import AdminSideBarComponent from "../../components/admin/AdminSideBarComponent";

export default {
  name: "AdminPage",
  components: {AdminSideBarComponent, FooterComponent, HeaderComponent}
}
</script>

<style lang="scss" scoped>

</style>