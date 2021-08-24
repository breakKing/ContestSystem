<template>
  <tasks-navigation-component :active_task_id="active_task_id" :tasks="tasks"
                              :mapped_solutions="mapped_solutions"></tasks-navigation-component>
  <chat-list-component route_name="ContestParticipatingChattingPage" :active_chat_id="active_chat_id" :chats="chats"></chat-list-component>

</template>

<script>
import ChatListComponent from "../../../chats/ChatListComponent";
import TasksNavigationComponent from "./TasksNavigationComponent";
import * as _ from "lodash";

export default {
  name: "ContestSideBarComponent",
  components: {
    ChatListComponent,
    TasksNavigationComponent,
  },
  props: {
    active_task_id: Number,
    active_chat_id: Number,
    mapped_solutions: Object,
    tasks: Array,
    chats: Array
  },
  computed:{
    actualTaskId() {
      let id
      if (this.active_task_id) {
        id = +this.active_task_id
      } else {
        id = +_.first(this.tasks)?.problemId || null
      }
      return id
    },
  }
}
</script>

<style scoped>

</style>