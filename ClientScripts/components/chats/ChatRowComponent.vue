<template>
  <template v-if="isEvent">
    <chat-event-component :event="row_instance"
                          v-if="show_events"></chat-event-component>
  </template>
  <chat-message-component :message="row_instance" v-else></chat-message-component>
</template>

<script>
import ChatEventComponent from "./rows/ChatEventComponent";
import ChatMessageComponent from "./rows/ChatMessageComponent";
import ChatEventTypes from "../../dictionaries/ChatEventTypes";

export default {
  name: "ChatRowComponent",
  components: {ChatMessageComponent, ChatEventComponent},
  props: {
    row_instance: Object,
    show_events: Boolean
  },
  computed: {
    isEvent() {
      return +this.row_instance?.type !== ChatEventTypes.Undefined && !this.row_instance?.text
    },
  }
}
</script>

<style lang="scss" scoped>

</style>