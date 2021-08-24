<template>
  <table class="table">
    <thead>
    <tr>
      <th>Действия</th>
    </tr>
    </thead>
    <tbody>
    <tr v-for="user of users">
      <td>
        <a @click.prevent="kickUser(user)">Исключить</a>
      </td>
    </tr>
    </tbody>
  </table>
</template>

<script>
export default {
  name: "ContestUsersListComponent",
  props: {
    users: Array
  },
  methods: {
    kickUser(user) {
      
    }
  }
}
</script>

<style scoped>

</style>