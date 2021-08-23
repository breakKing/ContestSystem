<template>
  <div class="row">
    <div class="col-12">
      <div class="messages-block p-2">
        <button v-if="chat_rows && chat_rows.length >= 50" @click="loadMoreMessages">Загрузить ещё</button>
        <chat-row-component :row_instance="row" v-for="row of chat_rows"></chat-row-component>
      </div>
    </div>
    <div class="col-12">
      <textarea class="form-control" placeholder="Введите сообщение" rows="3"></textarea>
      <div class="d-flex justify-content-end mt-4">
        <button class="btn btn-primary" @click="sendNewMessage">Отправить</button>
      </div>
    </div>
  </div>
</template>

<script>
import ChatRowComponent from "./ChatRowComponent";

export default {
  name: "ChatWindowComponent",
  components: {ChatRowComponent},
  props: {
    chat: Object
  },
  computed: {
    chat_rows() {
      return (this.chat?.historyEntries || [])
    }
  },
  methods: {
    loadMoreMessages() {

    },
    sendNewMessage() {
      
    }
  }
}
</script>

<style lang="scss" scoped>
.messages-block {
  height: 80vh;
  overflow-y: auto;
}
</style>