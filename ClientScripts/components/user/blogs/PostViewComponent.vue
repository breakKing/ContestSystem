﻿<template>
  <div class="my-3">
    <div class="container">
      <bread-crumbs-component :routes="bread_crumb_routes"></bread-crumbs-component>
      <div class="row">
        <div class="col-10 ms-auto me-auto">
          <div class="row">
            <div class="col-12 col-md-9">
              <h2>{{ post_info && post_info.localizedName }}</h2>
              <hr>
            </div>
            <div class="col">
              <p>{{ post_info && post_info.author && post_info.author.fullName }}</p>
              <p>{{ formatted_pub_date }}</p>
            </div>
          </div>
          <div class="row">
            <div class="col">
              <img class="img-fluid" :src="dataUrl" :alt="post_info && post_info.localizedName">
            </div>
          </div>
          <div class="row">
            <div class="col" v-html="post_info && post_info.htmlLocalizedText"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import {mapActions, mapGetters} from 'vuex'
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
    ...mapGetters(['getFormattedFullDateTime']),
    dataUrl() {
      if (!this.post_info || !this.post_info?.previewImage) {
        return '';
      }
      // загружено новое фото
      if (Array.isArray(this.post_info.previewImage)) {
        const [file] = this.post_info.previewImage
        if (file) {
          return URL.createObjectURL(file)
        }
      }
      return 'data:image/jpeg;base64,' + this.post_info.previewImage
    },
    bread_crumb_routes() {
      return PostViewBreadCrumbs(this.post_id)
    },
    formatted_pub_date() {
      return this.getFormattedFullDateTime(this.post_info?.publicationDateTimeUTC)
    }
  },
  methods: {
    ...mapActions(['getLocalizedPost']),
  },
  beforeRouteEnter(to, from, next) {
    next(async vm => {
      vm.post_info = await vm.getLocalizedPost(vm.post_id)
    })
  },
}
</script>

<style lang="scss" scoped>

</style>