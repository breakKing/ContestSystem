<template>
  <div class="row">
    <div class="col-12">
      <div class="messages-block p-2" v-if="filtered_chat_rows && filtered_chat_rows.length">
        <div class="d-flex justify-content-center mb-3">
          <button class="btn btn-primary" v-if="chat_rows && chat_rows.length >= 50" @click.prevent="loadMoreMessages">
            Загрузить ещё
          </button>
        </div>
        <chat-row-component :row_instance="row" :show_events="show_events"
                            v-for="row of filtered_chat_rows"></chat-row-component>
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
import ChatTypes from "../../dictionaries/ChatTypes";
import ChatEventTypes from "../../dictionaries/ChatEventTypes";

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
    },
    show_events() {
      return +this.chat?.type === ChatTypes.Custom
    },
    filtered_chat_rows() {
      return _.filter(this.chat_rows, (r) => {
        if (this.show_events) {
          return true
        }
        return !this.isEvent(r)
      })
    }
  },
  methods: {
    ...mapActions(['sendMessageToChat', 'fetchChatInfo']),
    isEvent(row) {
      return +row?.type !== ChatEventTypes.Undefined && !row?.text
    },
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
  max-height: 80vh;
  overflow-y: auto;
  border-radius: 0.3125rem;
  border: 1px solid black;
  margin-bottom: 1.5rem;
}
</style>