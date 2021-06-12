<template>
  <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
  <div class="container">
    <div class="row">
      <div class="col-12 col-md-9">
        <h2>{{ post_info?.localizedName }}</h2>
      </div>
      <div class="col">
        <p>{{ post_info?.author?.fullName }} {{ post_info?.publicationDateTimeUTC }}</p>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <img class="img-fluid" :src="dataUrl" :alt="post_info?.localizedName">
      </div>
    </div>
    <div class="row">
      <div class="col" v-html="post_info?.htmlLocalizedText"></div>
    </div>
  </div>
</template>

<script>
import {mapActions} from 'vuex'
import BreadCrumbsComponent from "../../BreadCrumbsComponent";
import PostViewBreadCrumbs from "../../../dictionaries/bread_crumbs/PostViewBreadCrumbs";

export default {
  name: "PostViewComponent",
  components: {BreadCrumbsComponent},
  props: ['post_id'],
  data() {
    return {
      post_info: null,
    }
  },
  computed: {
    dataUrl() {
      if (!this.post_info || !this.post_info?.previewImage) {
        return '';
      }
      return 'data:image/jpeg;base64,' + this.post_info.previewImage

    },
    bread_crumb_routes() {
      return PostViewBreadCrumbs(this.post_id)
    }
  },
  methods: {
    ...mapActions(['getPostInfo'])
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      vm.post_info = await vm.getPostInfo(vm.post_id)
    })
  },
}
</script>

<style lang="scss" scoped>

</style>