<template>
  <router-link class="btn btn-primary" :to="{name: 'WorkSpaceEditRuleSetPage'}">Создать</router-link>
  <rule-set-preview-component v-for="ruleSet of availableRuleSets" :rule-set="ruleSet"></rule-set-preview-component>
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

</style>