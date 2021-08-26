<template>
  <div class="row gx-0 mt-3">
    <rule-set-preview-component v-for="ruleSet of currentUserPendingRuleSets" :rule-set="ruleSet"></rule-set-preview-component>
  </div>
</template>

<script>
import RuleSetPreviewComponent from "./RuleSetPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "MyPendingRuleSetsListComponent",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters(['currentUserPendingRuleSets'])
  },
}
</script>

<style lang="scss" scoped>

</style>