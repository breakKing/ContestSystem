<template>
  <div class="col-6 col-md-3 mx-3 mb-3">
    <div class="card">
      <div class="card-body d-flex flex-column justify-content-between">
        <h5 class="card-title">{{ taskLocalizedName }}</h5>
        <div class="row">
          <div class="col w-50">
            <p>Ограничение по памяти</p>
            <p> {{ getFormattedMemory((task && task.memoryLimitInBytes) || 0) }}</p>
          </div>
          <div class="col w-50">
            <p>Ограничение по времени</p>
            <p> {{ getFormattedTime((task && task.timeLimitInMilliseconds) || 0) }}</p>
          </div>
        </div>
        <p> Автор: {{ task && task.creator && task.creator.fullName }}</p>
        <div class="row d-flex justify-content-center">
          <template v-if="currentRole === 'user'">
            <router-link v-if="currentUserIsOwner" :to="{name: 'WorkSpaceEditTaskPage', params: {task_id: task.id}}"
                         class="workspace-btn me-2">
              Редактировать
            </router-link>
          </template>
          <template v-else-if="currentRole === 'moderator'">
            <button @click.prevent="moderateTask" class="workspace-btn">Подробнее</button>
          </template>
          <button v-if="currentUserIsOwner" class="workspace-btn workspace-btn-del"
                  @click.prevent="deleteEntity">
            Удалить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import TaskEditComponent from "./TaskEditComponent";
import {mapGetters, mapActions} from "vuex";
import * as _ from 'lodash'

export default {
  name: "TaskPreviewComponent",
  components: {TaskEditComponent},
  props: {
    task: Object,
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole', 'getFormattedMemory', 'getFormattedTime']),
    currentUserIsOwner() {
      if (!this.currentUser || !this.task?.creator) {
        return false
      }
      return +this.task.creator.id === +this.currentUser.id
    },
    taskLocalizedName() {
      if (!this.task) {
        return ''
      }
      if (!this.task.localizedName) {
        return _.filter((this.task.localizers || []), (el) => el.culture === 'ru')[0].name
      }
      return this.task.localizedName
    }
  },
  methods: {
    ...mapActions(['fetchCurrentUserTasks',
      'fetchAvailableTasks',
      'deleteTask']),
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteTask(this.task?.id)
      if (status) {
        await this.fetchData()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async moderateTask() {
      await this.$router.push({
        name: 'ModeratorProblemModerationPage',
        params: {
          problem_id: +this.task.id
        }
      })
    },
    async fetchData() {
      await this.fetchCurrentUserTasks(true)
      await this.fetchAvailableTasks(true)
    }
  },
}
</script>

<style lang="scss" scoped>
.card {
  min-height: 230px;
  text-align: center;
  border: 1px solid blue;
}
</style>