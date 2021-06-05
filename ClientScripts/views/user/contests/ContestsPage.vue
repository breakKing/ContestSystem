<template>
  <ul class="nav nav-pills mb-3" role="tablist">
    <li class="nav-item" role="presentation">
      <button class="nav-link" :class="{active: $route.name === 'ParticipatingContestsPage'}"
              type="button" role="tab" :aria-selected="$route.name === 'ParticipatingContestsPage'">Учавствую
      </button>
    </li>
    <li class="nav-item" role="presentation">
      <button class="nav-link" :class="{active: $route.name === 'AvailableContestsPage'}"
              type="button" role="tab" :aria-selected="$route.name === 'AvailableContestsPage'">Доступные мне
      </button>
    </li>
    <li class="nav-item" role="presentation">
      <button class="nav-link" :class="{active: $route.name === 'CurrentlyRunningContestsComponentPage'}"
              type="button" role="tab" :aria-selected="$route.name === 'CurrentlyRunningContestsComponentPage'">Идут
        сейчас
      </button>
    </li>
  </ul>
  <!-- eslint-disable-next-line -->
  <div class="tab-content">
    <div class="tab-pane fade show active" role="tabpanel">
      <transition name="fade">
        <router-view></router-view>
      </transition>
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