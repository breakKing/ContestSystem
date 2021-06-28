<template>
  <ul class="nav nav-pills mb-3" role="tablist">
    <li class="nav-item" role="presentation">
      <router-link class="nav-link" :to="{name: 'ParticipatingContestsPage'}">Участвую</router-link>
    </li>
    <li class="nav-item" role="presentation">
      <router-link class="nav-link" :to="{name: 'AvailableContestsPage'}">Доступные мне</router-link>
    </li>
    <li class="nav-item" role="presentation">
      <router-link class="nav-link" :to="{name: 'CurrentlyRunningContestsComponentPage'}">Идут сейчас</router-link>
    </li>
  </ul>
  <!-- eslint-disable-next-line -->
  <div class="tab-content">
    <div class="tab-pane fade show active" role="tabpanel">
      <router-view></router-view>
    </div>
  </div>
</template>

<script>

export default {
  name: "ContestsPage",
}
</script>

<style lang="scss" scoped>

</style>