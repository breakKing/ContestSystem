<template>
  <rule-set-preview-component v-for="rule of ruleSetsToModerate" :encode_html="true" :rule-set="rule"></rule-set-preview-component>
</template>

<script>
import CheckerPreviewComponent from "../../../components/user/workspace/checkers/CheckerPreviewComponent";
import {mapActions, mapGetters} from "vuex";
import RuleSetPreviewComponent from "../../../components/user/workspace/rule_sets/RuleSetPreviewComponent";

export default {
  name: "ModeratorNotModeratedRuleSetsPage",
  components: {RuleSetPreviewComponent},
  computed: {
    ...mapGetters('moder_rule_sets', ['ruleSetsToModerate'])
  },
  methods: {
    ...mapActions('moder_rule_sets', ['fetchRuleSetsToModerate'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchRuleSetsToModerate()
    })
  },
}
</script>

<style scoped>

</style>