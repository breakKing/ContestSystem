<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <div class="row">
    <div class="col-12 col-md-3">
      <contest-side-bar-component
          :mapped_solutions="mappedSolutionsForCurrentContest"
          :tasks="orderedTasks"
          :chats="currentChats"
          :active_chat_id="chat_id"
      ></contest-side-bar-component>
    </div>
    <div class="col">
      <chat-window-component
          v-if="currentChat"
          :chat="currentChat"
      ></chat-window-component>
    </div>
  </div>
</template>
<script>
import ContestSideBarComponent from "./ContestSideBarComponent";
import ChatListComponent from "../../../chats/ChatListComponent";
import ChatWindowComponent from "../../../chats/ChatWindowComponent";
import BreadCrumbsComponent from "../../../BreadCrumbsComponent";
import ContestPageBreads from "../../../../dictionaries/bread_crumbs/contest/ContestPageBreads";
import {mapActions, mapGetters} from "vuex";
import * as _ from "lodash";

export default {
  name: "ContestChatComponent",
  props: ['contest_id', 'chat_id'],
  components: {
    BreadCrumbsComponent,
    ChatWindowComponent,
    ContestSideBarComponent,
    ChatListComponent,
  },
  computed: {
    ...mapGetters([
      'currentContest',
      'currentUser',
      'currentContestParticipants',
      'currentContestMonitorEntries',
      'currentUserIsOwnerOfCurrentContest',
      'currentUserIsParticipantOfCurrentContest',
      'currentContestIsRunning',
      'currentContestIsInPast',
      'currentContestIsInTheFuture',
      'mappedSolutionsForCurrentContest',
      'getContestChats',
    ]),
    bread_crumb_routes() {
      return ContestPageBreads(this.contest_id)
    },
    orderedTasks() {
      return _.sortBy((this.currentContest?.problems || []), ['letter'])
    },
    currentChats() {
      return this.getContestChats(this.currentContest.id)
    },
    currentChat() {
      return _.find(this.currentChats, (c) => +c.id === +this.chat_id)
    }
  },
  methods: {
    ...mapActions(['changeCurrentContest']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      vm.loading = true
      await vm.changeCurrentContest({force: false, contest_id: vm.contest_id})
      if (vm.currentContest && vm.currentContestIsInPast) {
        return await vm.$router.replace({name: 'ContestPage', params: {contest_id: vm.currentContest.id}})
      }
    })
  },
}
</script>

<style scoped>

</style>