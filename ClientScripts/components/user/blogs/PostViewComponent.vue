<template>
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
        <img :src="dataUrl" :alt="post_info?.localizedName">
      </div>
    </div>
    <div class="row">
      <div class="col" v-html="post_info?.htmlLocalizedText"></div>
    </div>
  </div>
  <!-- todo comments -->
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import {Base64} from 'js-base64';

export default {
  name: "PostViewComponent",
  props: ['post_id'],
  data() {
    return {
      post_info: null,
    }
  },
  computed: {
    dataUrl() {
      if (!this.post_info) {
        return '';
      }
      return 'data:image/jpeg;base64,' + this.post_info?.previewImage

    },
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