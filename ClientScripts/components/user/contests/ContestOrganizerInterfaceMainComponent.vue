<template>

</template>

<script>
import {mapGetters} from "vuex";

export default {
  name: "ContestOrganizerInterfaceMainComponent",
  props: {
    contest: Object,
  },
  computed: {
    ...mapGetters(['currentUser'])
  }
}
</script>

<style lang="scss" scoped>

</style>