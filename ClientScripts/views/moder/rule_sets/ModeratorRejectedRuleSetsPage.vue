<template>
  <rule-set-preview-component v-for="rule of rejectedRuleSet" :rule-set="rule"
                              :encode_html="true"></rule-set-preview-component>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import RuleSetPreviewComponent from "../../../components/user/workspace/rule_sets/RuleSetPreviewComponent";

export default {
  name: "ModeratorRejectedRuleSetsPage",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters('moder_rule_sets', ['rejectedRuleSet'])
  },
  methods: {
    ...mapActions('moder_rule_sets', ['fetchRejectedRuleSet'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRejectedRuleSet()
    })
  },
}
</script>

<style scoped>

</style>