<template>
  <table class="table">
    <thead>
    <tr>
      <th colspan="2">Задания</th>
      <th>
        <button class="btn btn-success" @click.prevent="$emit('update:tasks', {
          type: 'add',
          letter: getNextLetter()
        })">Добавить
        </button>
      </th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="(task, sym_idx) of (tasks || [])">
      <td>{{ task.letter }}</td>
      <td>
        <select class="form-control" @change="$emit('update:tasks', {
          type: 'changed',
          letter: task.letter,
          problemId: +$event.target.value
        })" v-model="selected_ids[sym_idx]">
          <option v-for="available_task of availableTasksForContest"
                  :value="+available_task.id">{{ available_task.localizedName }} {{ shouldTaskBeRemarked(available_task) ? '*' : ''}}
          </option>
        </select>
      </td>
      <td>
        <button class="btn btn-danger" @click.prevent="$emit('update:tasks', {
          type: 'delete',
          letter: task.letter
        })">Удалить
        </button>
      </td>
    </tr>
    </tbody>
  </table>
   <p v-if="unavailableTasksInFutureExists">* Данная задача более недоступна. Однако Вы можете использовать её для этого соревнования до тех пор, пока не замените её.</p>
</template>

<script>
import alphabet from 'alphabet'
import {mapActions, mapGetters} from "vuex";
import * as _ from 'lodash'

export default {
  name: "TasksSelectorComponent",
  props: ['tasks', 'availableTasksForContest'],
  emits: ['update:tasks'],
  methods: {
    getNextLetter() {
      if (!this.tasks) {
        return alphabet.upper[0]
      }
      return alphabet.upper[this.tasks.length]
    },
    shouldTaskBeRemarked(task) {
      let remark = false
      if (task) {
        if ((task.creator?.id != this.currentUser.id && !task.isPublic) || task.isArchieved) {
          remark = true
        }
      }
      return remark
    }
  },
  computed: {
    ...mapGetters(['currentUser']),
    unavailableTasksInFutureExists() {
      return _.reduce(this.availableTasksForContest || [], (count, t) => count += +this.shouldTaskBeRemarked(t), 0) > 0
    },
    selected_ids() {
      return _.cloneDeep(_.map((this.tasks || []), (t) => +t?.problemId))
    },
  },
}
</script>

<style lang="scss" scoped>

</style>