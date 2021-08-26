<template>
  <div class="row gx-0 mt-3">
    <rule-set-preview-component v-for="ruleSet of currentUserRejectedRuleSets" :rule-set="ruleSet"></rule-set-preview-component>
  </div>
</template>

<script>
import RuleSetPreviewComponent from "./RuleSetPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "MyRejectedRuleSetsListComponent",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters(['currentUserRejectedRuleSets'])
  },
}
</script>

<style lang="scss" scoped>

</style>