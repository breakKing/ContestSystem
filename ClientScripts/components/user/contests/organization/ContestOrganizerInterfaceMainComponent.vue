<template>
  <ul class="nav nav-pills">
    <li class="nav-item">
      <a class="nav-link" @click.prevent="active_page='solutions'" :class="{active: active_page === 'solutions'}"
         aria-current="page" href="#">Решения</a>
    </li>
    <li class="nav-item">
      <a class="nav-link" @click.prevent="active_page='users'" :class="{active: active_page === 'users'}"
         aria-current="page" href="#">Участники</a>
    </li>
    <li class="nav-item">
      <a class="nav-link" @click.prevent="active_page='chats'" :class="{active: active_page === 'chats'}"
         aria-current="page" href="#">Чаты</a>
    </li>
  </ul>
  <div v-if="active_page === 'solutions'">
    <contest-solutions-list-component
        :solutions="[]"
        :contest="currentContest"
        :organizer_mode="true"
    ></contest-solutions-list-component>
  </div>
  <div v-else-if="active_page === 'users'">
    <contest-users-list-component
        :users="[]"
    ></contest-users-list-component>
  </div>
  <div v-else-if="active_page === 'chats'">
    <div class="row">
      <div class="col-12 col-md-3">
        <chat-list-component
            @chat_selected="active_chat_id = $event"
        ></chat-list-component>
      </div>
      <div class="col">
        <chat-window-component
            v-if="active_chat_id"
            :chat="currentChat"
        >
        </chat-window-component>
      </div>
    </div>
  </div>
</template>

<script>
import {mapGetters} from "vuex";
import ContestSolutionsListComponent from "../participating/ContestSolutionsListComponent";
import ChatListComponent from "../../../chats/ChatListComponent";
import ChatWindowComponent from "../../../chats/ChatWindowComponent";
import * as _ from "lodash";
import ContestUsersListComponent from "./ContestUsersListComponent";

export default {
  name: "ContestOrganizerInterfaceMainComponent",
  components: {ContestUsersListComponent, ChatWindowComponent, ChatListComponent, ContestSolutionsListComponent},
  props: {
    contest: Object,
  },
  data() {
    return {
      active_chat_id: null,
      active_page: 'solutions' // users, chats
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'currentContest']),
    currentChats() {
      return this.getContestChats(this.currentContest?.id)
    },
    currentChat() {
      if (!this.active_chat_id) {
        return null
      }
      return _.find(this.currentChats, (c) => +c.id === +this.active_chat_id)
    }
  }
}
</script>

<style lang="scss" scoped>

</style>