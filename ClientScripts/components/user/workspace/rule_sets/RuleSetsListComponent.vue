<template>

    <div class="row mt-3">
    <rule-set-preview-component v-for="ruleSet of currentUserRuleSets" :rule-set="ruleSet"></rule-set-preview-component>
    </div>
</template>

<script>
import RuleSetPreviewComponent from "./RuleSetPreviewComponent";
import {mapActions, mapGetters} from "vuex";

export default {
  name: "RuleSetsListComponent",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters(['currentUserRuleSets'])
  },
}
</script>

<style lang="scss" scoped>

</style>