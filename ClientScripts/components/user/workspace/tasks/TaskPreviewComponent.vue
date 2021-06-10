<template>
  <div class="row">
    <div class="col">
      <div class="row">
        <div class="col">
          <p>Название</p>
          <p> {{ task?.name }}</p>
        </div>
        <div class="col">
          <p>Описание</p>
          <p> {{ task?.localizedDescription }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col">
          <p>Входные данные</p>
          <p> {{ task?.localizedInputBlock }}</p>
        </div>
        <div class="col">
          <p>Выходные данные</p>
          <p> {{ task?.localizedOutputBlock }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col">
          <p>Ограничение по памяти</p>
          <p> {{ task?.memoryLimitInBytes }}</p>
        </div>
        <div class="col">
          <p>Ограничение по времени</p>
          <p> {{ task?.timeLimitInMilliseconds }}</p>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-3">
      <router-link v-if="currentUserIsOwner" :to="{name: 'WorkSpaceEditTaskPage', params: {task_id: this.task?.id}}"
                   class="btn btn-info">Редактировать
      </router-link>
    </div>
  </div>
</template>

<script>
import TaskEditComponent from "./TaskEditComponent";
import {mapGetters} from "vuex";

export default {
  name: "TaskPreviewComponent",
  components: {TaskEditComponent},
  props: {
    task: Object,
  },
  computed: {
    ...mapGetters(['currentUser']),
    currentUserIsOwner() {
      if (!this.currentUser || !this.task?.creator) {
        return false
      }
      return +this.task.creator.id === +this.currentUser.id
    }
  }
}
</script>

<style lang="scss" scoped>

</style>