<template>
  <div class="row">
    <div class="col-12 col-md-3">
      {{ ruleSet.name }}
    </div>
    <div class="col-12 col-md-7">
      {{ ruleSet.description }}
    </div>
    <div class="col-12 col-md-2">
      <button class="btn btn-primary"
              @click="$router.push({name: 'WorkSpaceEditRuleSetPage',params:{set_id: ruleSet.id}})">Редактировать
      </button>
    </div>
  </div>
</template>

<script>

export default {
  name: "RuleSetPreviewComponent",
  props: {
    ruleSet: Object,
  },
}

</script>

<style lang="scss" scoped>

</style>