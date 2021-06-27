<template>
    <!--eslint-disable -->
    <router-link :to="{name: 'WorkSpaceEditCheckersPage'}" class="workspace-btn">Создать</router-link>
    <div class="row mt-4">
        <checker-preview-component v-for="checker of availableCheckers" :checker="checker"></checker-preview-component>
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
        padding: 5px 10px;
        background-color: #fff;
        border-radius: 16px;
        border: 1px solid blue;
        text-decoration: none;

        &:hover {
            background-color: #0b76ef;
            color: white;
        }
    }
</style>