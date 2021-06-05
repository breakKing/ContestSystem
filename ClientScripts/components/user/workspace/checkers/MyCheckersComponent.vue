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
  watch: {
    async $route(to, from) {
      await this.fetchAvailableCheckers()
    }
  },
  async created() {
    await this.fetchAvailableCheckers(true)
  },
}
</script>

<style lang="scss" scoped>

</style>