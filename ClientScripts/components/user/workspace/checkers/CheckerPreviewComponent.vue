<template>
  <div class="row">
    <div class="col-12 col-md-3">
      {{ checker.name }}
    </div>
    <div class="col-12 col-md-7">
      {{ checker.description }}
    </div>
    <div class="col-12 col-md">
      <button class="btn btn-primary"
              @click.prevent="$router.push({name: 'WorkSpaceEditCheckersPage', params: {id: Number(checker.id)}})">Редактировать
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: "CheckerPreviewComponent",
  props: {
    checker: Object
  }
}
</script>

<style lang="scss" scoped>

</style>