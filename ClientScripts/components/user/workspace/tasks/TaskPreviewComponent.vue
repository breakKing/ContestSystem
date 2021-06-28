<template>
    <div class="col-md-6 mb-3">
        <div class="card">
            <div class="card-body d-flex flex-column justify-content-between">
                <h5 class="card-title">{{ task?.localizedName }}</h5>
                <div class="row">
                    <div class="col w-50">
                        <p>Ограничение по памяти</p>
                        <p> {{ task?.memoryLimitInBytes }}</p>
                    </div>
                    <div class="col w-50">
                        <p>Ограничение по времени</p>
                        <p> {{ task?.timeLimitInMilliseconds }}</p>
                    </div>
                </div>
                <p> Автор: {{task?.сreator?.fullName}}</p>
                <div class="row d-flex justify-content-center">
                    <template v-if="currentRole === 'user'">
                        <router-link v-if="currentUserIsOwner" :to="{name: 'WorkSpaceEditTaskPage', params: {task_id: task?.id}}"
                                     class="workspace-btn me-2">
                            Редактировать
                        </router-link>
                    </template>
                    <template v-else-if="currentRole === 'moderator'">
                        <button @click.prevent="moderateTask" class="workspace-btn">Подробнее</button>
                    </template>
                    <button v-if="currentUserIsOwner" class="workspace-btn workspace-btn-del"
                            @click.prevent="shittyDeleteFunction">
                        Удалить
                    </button>
                </div>
            </div>
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
    ...mapGetters(['currentUser', 'currentRole']),
    currentUserIsOwner() {
      if (!this.currentUser || !this.task?.creator) {
        return false
      }
      return +this.task.creator.id === +this.currentUser.id
    }
  },
  methods: {
    async moderateTask() {
      await this.$router.push({
        name: 'ModeratorProblemModerationPage',
        params: {
          problem_id: +this.task.id
        }
      })
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