<template>
  <rule-set-preview-component v-for="rule of approvedRuleSet" :encode_html="true" :rule-set="rule"></rule-set-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import CheckerPreviewComponent from "../../../components/user/workspace/checkers/CheckerPreviewComponent";
import RuleSetPreviewComponent from "../../../components/user/workspace/rule_sets/RuleSetPreviewComponent";

export default {
  name: "ModeratorApprovedRuleSetsPage",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters('moder_rule_sets', ['approvedRuleSet'])
  },
  methods: {
    ...mapActions('moder_rule_sets', ['fetchApprovedRuleSet'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchApprovedRuleSet()
    })
  },
}
</script>

<style scoped>

</style>