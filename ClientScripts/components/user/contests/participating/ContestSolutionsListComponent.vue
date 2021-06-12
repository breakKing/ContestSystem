<template>
  <table class="table">
    <thead>
    <tr>
      <th>Задача</th>
      <th>Компилятор</th>
      <th>Время отправки</th>
      <th>Вердикт</th>
      <th>Очки</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="solution of sortedSolutions" :class="{current: +solution.id === +solution_id}">
      <td>{{ solution.problem.localizedName }}</td>
      <td>{{ solution.compilerName }}</td>
      <td>{{ solution.submitTimeUTC }}</td>
      <td>{{ solution.verdict }}</td>
      <td>{{ solution.points }}</td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from "vuex";
import * as _ from 'lodash'

export default {
  name: "ContestSolutionsListComponent",
  props: ['contest_id', 'solution_id'],
  data() {
    return {
      interval: null,
    }
  },
  computed: {
    ...mapGetters(['currentUser', 'currentContestSolutionsForCurrentUser']),
    sortedSolutions() {
      return _.sortBy(this.currentContestSolutionsForCurrentUser, (s) => +s.id === +this.solution_id)
    }
  },
  methods: {
    ...mapActions(['getUserSolutionsInContest']),
    ...mapMutations(['setCurrentContestSolutionsForCurrentUser']),
    async updateList() {
      let solutions = await this.getUserSolutionsInContest({
        contest_id: this.contest_id,
        user_id: this.currentUser?.id
      })
      this.setCurrentContestSolutionsForCurrentUser(solutions)
    },
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      await vm.getUserSolutionsInContest({
        contest_id: vm.contest_id,
        user_id: vm.currentUser?.id
      })

      if (!vm.interval) {
        vm.interval = setInterval(vm.updateList, 5000)
      }
    })
  },
  mounted() {
    if (!this.interval) {
      this.interval = setInterval(this.updateList, 5000)
    }
  },
  unmounted() {
    if (this.interval) {
      clearInterval(this.interval)
    }
  },
  beforeRouteLeave(to, from, next) {
    if (this.interval) {
      clearInterval(this.interval)
    }
    next()
  }
}
</script>

<style lang="scss" scoped>

</style>