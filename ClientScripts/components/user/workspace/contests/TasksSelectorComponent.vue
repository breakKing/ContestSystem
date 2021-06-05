<template>
  <table class="table">
    <thead>
    <tr>
      <th colspan="2">Задания</th>
      <th>
        <button class="btn btn-success" @click="$emit('update:tasks', {
          type: 'add',
          letter: getNextLetter()
        })">Добавить
        </button>
      </th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="(task, sym_idx) of (tasks||[])">
      <td>{{ task.letter }}</td>
      <td>
        <select class="form-control" @change="$emit('update:tasks', {
          type: 'changed',
          letter: task.letter,
          problemId: $event.target.value
        })" v-model="selected_ids[sym_idx]">
          <option v-for="available_task of availableTasks"
                  :value="available_task.id">{{ available_task.localizedName }}
          </option>
        </select>
      </td>
      <td>
        <button class="btn btn-danger" @click="selected_ids.splice(sym_idx,1);$emit('update:tasks', {
          type: 'delete',
          letter: task.letter
        })">Удалить
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import alphabet from 'alphabet'
import {mapActions, mapGetters} from "vuex";
import * as _ from 'lodash'

export default {
  name: "TasksSelectorComponent",
  props: ['tasks'],
  emits: ['update:tasks'],
  data() {
    return {
      selected_ids: []
    }
  },
  methods: {
    ...mapActions(['fetchAvailableTasks']),
    getNextLetter() {
      if (!this.tasks) {
        return alphabet.upper[0]
      }
      return alphabet.upper[this.tasks.length]
    },
    logThis(data) {
      console.log(data)
    }
  },
  computed: {
    ...mapGetters(['availableTasks']),
  },
  watch: {
    async $route(to, from) {
      await this.fetchAvailableTasks()
      this.selected_ids = _.map(this.tasks, (t) => t?.problemId)
    }
  },
  async mounted() {
    await this.fetchAvailableTasks(true)
    this.selected_ids = _.map(this.tasks, (t) => t?.problemId)
  },

}
</script>

<style lang="scss" scoped>

</style>