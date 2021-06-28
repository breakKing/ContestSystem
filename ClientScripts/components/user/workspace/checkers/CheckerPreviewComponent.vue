<template>
    <div class="col-md-4 mb-3">
        <div class="card">
            <div class="card-body d-flex flex-column justify-content-between">
                <h5 class="card-title">{{ checker.name }}</h5>
                <p class="card-text">{{ checker.description }}</p>
                <p> Автор: {{checker.author.fullName}}</p>
                <div class="row d-flex justify-content-between">
                    <button v-if="currentRole === 'user' && +currentUser?.id === +checker.author.id" class="workspace-btn me-2"
                            @click.prevent="$router.push({name: 'WorkSpaceEditCheckersPage', params: {id: +checker.id }})">
                        Редактировать
                    </button>
                    <button v-else-if="currentRole === 'moderator'" class="workspace-btn"
                            @click.prevent="moderateChecker">
                        Подробнее
                    </button>
                    <button v-if="+currentUser?.id === +checker.author.id" class="workspace-btn workspace-btn-del"
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
  name: "CheckerPreviewComponent",
  props: {
    checker: Object
  },
  computed: {
    ...mapGetters(['currentUser', 'currentRole'])
  },
  methods: {
    async moderateChecker() {
      await this.$router.push({
        name: 'ModeratorCheckerModerationPage',
        params: {
          checker_id: +this.checker.id
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>
    .card {
        min-height: 230px;
        text-align: center;
        border: 1px solid blue;
    }

    .workspace-btn {
        padding: 5px 5px;
        background-color: #fff;
        border-radius: 16px;
        border: 1px solid blue;
        text-decoration: none;
        color: blue;
        width: 48%;

        &:hover {
            background-color: #0b76ef;
            color: white;
        }
    }

    .workspace-btn-del {
        border: 1px solid red;
        color: red;

        &:hover {
            background-color: red;
            color: white;
        }
    }
</style>