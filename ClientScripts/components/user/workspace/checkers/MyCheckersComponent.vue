<template>
  <!--eslint-disable -->
  <router-link :to="{name: 'WorkSpaceEditCheckersPage'}" class="btn btn-primary">Создать</router-link>
  <checker-preview-component v-for="checker of availableCheckers" :checker="checker">
  </checker-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import CheckerPreviewComponent from "./CheckerPreviewComponent";

export default {
  name: "MyCheckersComponent",
  components: {CheckerPreviewComponent},
  methods: {
    ...mapActions(['fetchAvailableCheckers'])
  },
  computed: {
    ...mapGetters(['availableCheckers'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAvailableCheckers()
    })
  },
}
</script>

<style lang="scss" scoped>

</style>