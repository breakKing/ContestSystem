<template>
  <div>
    <a-breadcrumb :routes="routes">
      <template #itemRender="{ route, routes }">
        <span v-if="routes.indexOf(route) === routes.length - 1">
          {{ route.breadcrumbName }}
        </span>
        <router-link v-else :to="route.to">
          {{ route.breadcrumbName }}
        </router-link>
      </template>
    </a-breadcrumb>
    <br />
  </div>
</template>
<script>
export default {
  name: 'BreadCrumbsComponent',
  props:['routes'],
};
</script>
