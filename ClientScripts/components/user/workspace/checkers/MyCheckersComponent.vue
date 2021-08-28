<template>
  <div class="p-3">
    <router-link :to="{name: 'WorkSpaceEditCheckersPage'}" class="workspace-btn">Создать</router-link>
    <div class="row gx-0 mt-3">
      <checker-preview-component v-for="checker of availableCheckers" :checker="checker"></checker-preview-component>
    </div>
  </div>
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
.workspace-btn {
  padding: 0.3125rem 0.625rem;
  background-color: #fff;
  border-radius: 1rem;
  border: 1px solid blue;
  text-decoration: none;

  &:hover {
    background-color: #0b76ef;
    color: white;
  }
}
</style>