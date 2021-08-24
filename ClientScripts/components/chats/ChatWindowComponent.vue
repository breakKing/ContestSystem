<template>
  <div class="row">
    <div class="col-12">
      <div class="messages-block p-2">
        <div class="d-flex justify-content-center">
          <button class="btn btn-primary" v-if="chat_rows && chat_rows.length >= 50" @click.prevent="loadMoreMessages">Загрузить ещё</button>
        </div>
        <chat-row-component :row_instance="row" v-for="row of chat_rows"></chat-row-component>
      </div>
    </div>
    <div class="col-12">
      <textarea v-model="message" class="form-control" placeholder="Введите сообщение" rows="3"></textarea>
      <div class="d-flex justify-content-end mt-4">
        <button class="btn btn-primary" @click.prevent="sendNewMessage">Отправить</button>
      </div>
    </div>
  </div>
</template>

<script>
import ChatRowComponent from "./ChatRowComponent";
import {mapActions} from "vuex";

export default {
  name: "ChatWindowComponent",
  components: {ChatRowComponent},
  props: {
    chat: Object
  },
  data() {
    return {
      message: '',
    }
  },
  computed: {
    chat_rows() {
      return (this.chat?.historyEntries || [])
    }
  },
  methods: {
    ...mapActions(['sendMessageToChat', 'fetchChatInfo']),
    async loadMoreMessages() {
      if (this.chat) {
        let link = this.chat.link;
        let offset = this.chat.historyEntries.length;
        await this.fetchChatInfo({
          link,
          offset
        })
      }
    },
    async sendNewMessage() {
      let text = this.message.trim()
      if (text) {
        await this.sendMessageToChat({
          chat: this.chat,
          text
        })
        this.message = ''
      }
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