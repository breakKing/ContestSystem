<template>
  <div class="row" @click.prevent="goToContest">
    <div class="col">
      <div class="row">
        <div class="col-8">
          {{ contest.localizedName }}
        </div>
        <div class="col">
          {{ contest.creator?.fullName }}
        </div>
      </div>
      <div class="row">
        <div class="col">
          {{ contest.participatsCount || 0 }} участников
        </div>
      </div>
      <div class="row">
        <div class="col">
          {{ contest.localizedDescription }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ContestPreviewComponent",
  props: {
    contest: Object
  },
  methods: {
    goToContest() {
      this.$router.push({name: 'WorkSpaceEditContestPage', params: {contest_id: this.contest.id}})
    }
  }
}
</script>

<style lang="scss" scoped>

</style>