<template>
  <chat-event-component :event="row_instance"
                        v-if="+row_instance.type !== 9 && !row_instance.text"></chat-event-component>
  <chat-message-component :message="row_instance" v-else></chat-message-component>
</template>

<script>
import ChatEventComponent from "./rows/ChatEventComponent";
import ChatMessageComponent from "./rows/ChatMessageComponent";

export default {
  name: "ChatRowComponent",
  components: {ChatMessageComponent, ChatEventComponent},
  props: {
    row_instance: Object
  }
}
</script>

<style lang="scss" scoped>

</style>