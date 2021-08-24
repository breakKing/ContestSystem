<template>
  <div class="d-flex flex-column">
    <h4>Чаты</h4>
    <template v-for="chat of chats">
      <a v-if="enable_event_mode" :class="{active: +active_chat_id === +chat.id}"
         @click.prevent="$emit('chat_selected', +chat.id)">{{
          chat.name
        }}</a>
      <router-link v-else :class="{active: +active_chat_id === +chat.id}"
                   :to="{name: route_name, params: {chat_id: chat.id}}">
        {{
          chat.name
        }}
      </router-link>
    </template>
  </div>
</template>

<script>
export default {
  name: "ChatListComponent",
  emits: ['chat_selected'],
  props: {
    chats: Array,
    route_name: String,
    active_chat_id: Number,
    enable_event_mode: {type: Boolean, default: false},
  }
}
</script>

<style lang="scss" scoped>

</style>