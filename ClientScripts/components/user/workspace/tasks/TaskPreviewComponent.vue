<template>
  <div class="row">
    <div class="col">
      <p>Название</p>
      <p> {{ task?.name }}</p>
    </div>
    <div class="col">
      <p>Описание</p>
      <p> {{ task?.localizedDescription }}</p>
    </div>
  </div>
  <div class="row">
    <div class="col">
      <p>Входные данные</p>
      <p> {{ task?.localizedInputBlock }}</p>
    </div>
    <div class="col">
      <p>Выходные данные</p>
      <p> {{ task?.localizedOutputBlock }}</p>
    </div>
  </div>
  <div class="row">
    <div class="col">
      <p>Ограничение по памяти</p>
      <p> {{ task?.memoryLimitInBytes }}</p>
    </div>
    <div class="col">
      <p>Ограничение по времени</p>
      <p> {{ task?.timeLimitInMilliseconds }}</p>
    </div>
  </div>
</template>

<script>
export default {
  name: "TaskPreviewComponent",
  props: {
    task: Object,
  }
}
</script>

<style lang="scss" scoped>

</style>