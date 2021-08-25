<template>
  <div class="d-flex flex-column">
    <h4>Чаты</h4>
    <template v-for="chat of chats">
      <div class="card mb-2 chat-card">
        <div class="card-body">
          <a v-if="enable_event_mode" class="chat-item no_style_link" :class="{active: +active_chat_id === +chat.id}"
             @click.prevent="$emit('chat_selected', +chat.id)">{{
              getChatName(chat)
            }}</a>
          <router-link v-else class="chat-item no_style_link" :class="{active: +active_chat_id === +chat.id}"
                       :to="{name: route_name, params: {chat_id: chat.id}}">
            {{
              getChatName(chat)
            }}
          </router-link>
        </div>
      </div>
    </template>
  </div>
</template>

<script>
import ChatTypes from "../../dictionaries/ChatTypes";
import {mapGetters} from "vuex";

export default {
  name: "ChatListComponent",
  emits: ['chat_selected'],
  props: {
    chats: Array,
    route_name: String,
    active_chat_id: Number,
    enable_event_mode: {type: Boolean, default: false},
  },
  computed: {
    ...mapGetters(['currentUser', 'currentContest', 'currentUserIsParticipantOfCurrentContest']),
  },
  methods: {
    isCurrentUserParticipantOfChatContest(contest_id) {
      if (!this.currentUser || !this.currentContest) {
        return false
      }
      if (+this.currentContest.id === +contest_id) {
        return this.currentUserIsParticipantOfCurrentContest
      }
      return false
    },
    getChatName(chat) {
      if (chat && +chat.type === ChatTypes.ContestParticipant && this.isCurrentUserParticipantOfChatContest(+chat.contestId)) {
        return 'Чат с организаторами'
      }
      return chat.name
    }
  }
}
</script>

<style lang="scss" scoped>
.card-body {
  padding: 0;
}

.chat-card {
  cursor: pointer;

  &:hover {
    background-color: #e0e0e0;
  }

  .chat-item {
    display: block;
    padding: 1rem;
  }
}
</style>