<template>
  <div class="row">
    <div class="col-12 col-md-3">
      {{ ruleSet.name }}
    </div>
    <div class="col-12 col-md-7">
      {{ ruleSet.description }}
    </div>
    <div class="col-12 col-md-2">
      <button v-if="+currentUser?.id === +ruleSet.author.id" class="btn btn-primary"
              @click.prevent="$router.push({name: 'WorkSpaceEditRuleSetPage', params: { set_id: ruleSet.id }})">Редактировать
      </button>
    </div>
  </div>
</template>

<script>

import {mapGetters} from "vuex";

export default {
  name: "RuleSetPreviewComponent",
  props: {
    ruleSet: Object,
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole'])
  }
}

</script>

<style lang="scss" scoped>

</style>