<template>
  <header-component></header-component>
  <div class="container">
    <router-view></router-view>
  </div>
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
