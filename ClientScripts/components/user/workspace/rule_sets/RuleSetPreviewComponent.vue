<template>
  <div class="col-md-4 mb-3">
    <div class="card">
      <div class="card-body d-flex flex-column justify-content-between">
        <h5 class="card-title">{{ ruleSet.name }}</h5>
        <template v-if="ruleSet && ruleSet.description">
          <p class="card-text" v-if="encode_html">{{ ruleSet.description }}</p>
          <p class="card-text" v-else v-html="ruleSet.description"></p>
        </template>
        <p> Автор: {{ ruleSet.author.fullName }}</p>
        <div class="row d-flex justify-content-between">
          <button v-if="currentUser && +currentUser.id === +ruleSet.author.id" class="workspace-btn me-2"
                  @click.prevent="$router.push({name: 'WorkSpaceEditRuleSetPage', params: { set_id: ruleSet.id }})">
            Редактировать
          </button>
          <button v-else-if="currentRole === 'moderator'" class="workspace-btn"
                  @click.prevent="moderateRuleSet">
            Подробнее
          </button>
          <button v-if="currentUser && +currentUser.id === +ruleSet.author.id" class="workspace-btn workspace-btn-del"
                  @click.prevent="deleteEntity">
            Удалить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>

import {mapGetters, mapActions} from "vuex";

export default {
  name: "RuleSetPreviewComponent",
  props: {
    ruleSet: Object,
    encode_html: {
      type: Boolean,
      default: false
    },
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole'])
  },
  methods: {
    ...mapActions(['deleteRuleSet',
      'fetchAvailableRuleSets',
      'fetchCurrentUserRuleSets']),
    async moderateRuleSet() {
      await this.$router.push({
        name: 'ModeratorRuleSetModerationPage',
        params: {
          rule_set_id: +this.ruleSet.id
        }
      })
    },
    async deleteEntity() {
      this.error_msg = ''
      let {status, errors} = await this.deleteRuleSet(this.ruleSet?.id)
      if (status) {
        await this.fetchData()
      } else {
        this.error_msg = (errors || []).join(', ')
      }
    },
    async fetchData() {
      await this.fetchCurrentUserRuleSets(true)
      await this.fetchAvailableRuleSets(true)
    }
  }
}

</script>

<style lang="scss" scoped>
.card {
  min-height: 210px;
  text-align: center;
  border: 1px solid blue;
}
</style>