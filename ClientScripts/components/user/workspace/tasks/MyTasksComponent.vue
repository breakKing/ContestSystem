<template>
  <div class="p-3">
    <router-link :to="{name: 'WorkSpaceEditTaskPage'}" class="workspace-btn">Создать задачу</router-link>
    <div class="row mt-4">
      <task-preview-component v-for="task of availableTasks" :task="task"></task-preview-component>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from "vuex";
import TaskPreviewComponent from "./TaskPreviewComponent";

export default {
  name: "MyTasksComponent",
  components: {TaskPreviewComponent},
  methods: {
    ...mapActions(['fetchAvailableTasks'])
  },
  computed: {
    ...mapGetters(['availableTasks'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.fetchAvailableTasks()
    })
  },
}
</script>

<style lang="scss" scoped>
.workspace-btn {
  padding: 5px 5px;
  background-color: #fff;
  border-radius: 16px;
  border: 1px solid blue;
  text-decoration: none;
  color: blue;
  width: 48%;

  &:hover {
    background-color: #0b76ef;
    color: white;
  }
}
</style>