<template>
<!--eslint-disable -->
<div class="container-fluid bg-dark fs-5 text-light">
  <div class="container py-3 d-flex align-items-center justify-content-between">
    <ul>
      <li>Божьев Владислав</li>
      <li>Духовный Даниил</li>
      <li>Жуков Дмитрий</li>
    </ul>
    <h5 class="text-light">&#169; 2021 - ContestSystem</h5>
    <router-link class="navbar-brand text-light" :to="{name: 'Home'}">
      ContestSystem
      <!--<img src="~../../Images/logo.png" alt="logo">-->
    </router-link>
  </div>
</div>
  
</template>

<script>
export default {
  name: "FooterComponent",
}
</script>

<style lang="scss" scoped>
.navbar-brand{
  font-family: cursive;
  padding: 15px;
  border-radius: 16px 0 16px 0;
  background-color: rgb(0, 13, 189, 0.2);
}

ul{
  list-style-type: none;
  padding-top: 10px;
  li{
    font-family: cursive;
    h3{
      font-family: cursive;
    }
  }
}
</style>