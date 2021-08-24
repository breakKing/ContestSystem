<template>
  <div class="d-flex justify-content-center">
    <!--    TODO текст в соответствии с типом события-->
    <div class="event-block">{{ event.dateTimeUTC }}</div>
  </div>
</template>

<script>
export default {
  name: "ChatEventComponent",
  props: {
    event: Object
  }
}
</script>

<style lang="scss" scoped>

</style>