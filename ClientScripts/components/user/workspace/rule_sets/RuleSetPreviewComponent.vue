<template>
    <div class="col-md-4 mb-3">
        <div class="card">
            <div class="card-body d-flex flex-column justify-content-between">
                <h5 class="card-title">{{ ruleSet.name }}</h5>
                <p class="card-text">{{ ruleSet.description }}</p>
                <p> Автор: {{ruleSet.author.fullName}}</p>
                <div class="row d-flex justify-content-between">
                    <button v-if="+currentUser?.id === +ruleSet.author.id" class="workspace-btn me-2"
                            @click.prevent="$router.push({name: 'WorkSpaceEditRuleSetPage', params: { set_id: ruleSet.id }})">
                        Редактировать
                    </button>
                    <button v-if="+currentUser?.id === +ruleSet.author.id" class="workspace-btn workspace-btn-del"
                            @click.prevent="shittyDeleteFunction">
                        Удалить
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

import {mapGetters} from "vuex";

export default {
  name: "RuleSetPreviewComponent",
  props: {
    ruleSet: Object,
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole'])
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