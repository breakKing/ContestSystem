<template>
  <router-link class="workspace-btn" :to="{name: 'WorkSpaceEditRuleSetPage'}">Создать</router-link>
  <div class="row gx-0 mt-3">
    <rule-set-preview-component v-for="ruleSet of availableRuleSets" :rule-set="ruleSet"></rule-set-preview-component>
  </div>
</template>

<script>
import RuleSetPreviewComponent from "./RuleSetPreviewComponent";
import {mapActions, mapGetters} from "vuex";
import RuleSetEditComponent from "./RuleSetEditComponent";

export default {
  name: "AllRuleSetsListComponent",
  components: {RuleSetEditComponent, RuleSetPreviewComponent},
  computed: {
    ...mapGetters(['availableRuleSets'])
  },
  methods: {
    ...mapActions(['fetchAvailableRuleSets'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAvailableRuleSets()
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