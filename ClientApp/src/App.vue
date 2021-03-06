<template>
  <nav-menu></nav-menu>
  <router-view />
</template>

<script>
    import NavMenu from './components/NavMenu.vue'

export default {
  name: 'App',
  components: {
      NavMenu
  }
}
</script>
