<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  Тут будут курсы :)
</template>

<script>


import BreadCrumbsComponent from "../../components/BreadCrumbsComponent";
import CoursesListBreads from "../../dictionaries/bread_crumbs/CoursesListBreads";

export default {
  name: "CoursePage",
  components: {BreadCrumbsComponent},
  computed: {
    bread_crumb_routes() {
      return CoursesListBreads
    }
  },
}
</script>

<style lang="scss" scoped>

</style>