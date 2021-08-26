<template>
  <div class="row mt-3">
    <rule-set-preview-component v-for="ruleSet of currentUserApprovedRuleSets" :rule-set="ruleSet"></rule-set-preview-component>
  </div>
</template>

<script>
import RuleSetPreviewComponent from "./RuleSetPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "MyApprovedRuleSetsListComponent",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters(['currentUserApprovedRuleSets'])
  },
}
</script>

<style lang="scss" scoped>

</style>