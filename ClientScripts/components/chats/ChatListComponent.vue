<template>
  <div class="d-flex flex-column">
    <h4>Чаты</h4>
    <router-link :class="{active: +active_chat_id === +chat.id}" :to="{name: route_name, params: {chat_id: chat.id}}" v-for="chat of chats">{{
        chat.name
      }}
    </router-link>
  </div>
</template>

<script>
export default {
  name: "ChatListComponent",
  props: {
    chats: Array,
    route_name: String,
    active_chat_id: Number,
  }
}
</script>

<style lang="scss" scoped>

</style>