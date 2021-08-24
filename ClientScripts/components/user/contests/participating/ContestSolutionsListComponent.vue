<template>
  <table class="table">
    <thead>
    <tr>
      <th>Задача</th>
      <th>Компилятор</th>
      <th>Время отправки</th>
      <th>Статус проверки</th>
      <th v-if="pointsAreCounted">Очки</th>
      <th>Время</th>
      <th>Память</th>
      <th>Подробности</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="solution of sortedSolutions">
      <td>
        <span v-if="organizer_mode"> {{ problemName(solution.problem) }}</span>
        <router-link v-else :to="{name: 'ContestParticipatingPage', params: {task_id: solution.problem.id}}">
          {{ problemName(solution.problem) }}
        </router-link>
      </td>
      <td>{{ solution.compilerName }}</td>
      <td>{{ getFormattedFullDateTime(solution.submitTimeUTC) }}</td>
      <td>{{ verdictInfo(actualResult(solution)) }}</td>
      <td v-if="pointsAreCounted">{{ (actualResult(solution) && actualResult(solution).points) || 0 }}</td>
      <td>{{ getFormattedTime((actualResult(solution) && actualResult(solution).usedTimeInMillis) || 0) }}</td>
      <td>{{ getFormattedMemory((actualResult(solution) && actualResult(solution).usedMemoryInBytes) || 0) }}</td>
      <td>
        <router-link :to="{name: 'SolutionViewPage', params: {solution_id: solution.id}}">Перейти</router-link>
      </td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import {mapGetters} from "vuex";
import * as _ from 'lodash'
import CountModes from "../../../../dictionaries/CountModes";
import solution_verdict_readable_presentation from "../../../mixins/solution_verdict_readable_presentation";

export default {
  name: "ContestSolutionsListComponent",
  mixins: [solution_verdict_readable_presentation],
  props: {
    solutions: Array,
    contest: Object,
    organizer_mode: Boolean,
  },
  computed: {
    ...mapGetters(['getFormattedFullDateTime', 'getFormattedMemory', 'getFormattedTime']),
    sortedSolutions() {
      return _.sortBy(this.solutions, (s) => -s.id)
    },
    pointsAreCounted() {
      return +this.contest?.rules?.countMode !== +CountModes.CountPenalty
    },
  },
  methods: {
    problemName(problem) {
      if (!problem) {
        return ''
      }
      let letter = _.find(this.contest?.problems || [], (p) => +p.problemId === +problem.id)?.letter + '. ' || ''
      return letter + (problem.localizedName || '')
    },
  },
}
</script>

<style lang="scss" scoped>

</style>