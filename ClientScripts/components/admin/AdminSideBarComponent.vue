<template>
  <ul class="list-group">
    <li class="list-group-item" :class="{active: $route.name === 'AdminUsersList'}"
        @click="$router.push({name: 'AdminUsersList'})">Пользователи
    </li>
  </ul>
</template>

<script>
export default {
  name: "AdminSideBarComponent",
}
</script>

<style lang="scss" scoped>
.list-group-item {
  &:hover {
    cursor: pointer;
  }
}
</style>