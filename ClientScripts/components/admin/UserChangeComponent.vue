<template>
  <button type="button" class="btn btn-primary" data-toggle="modal" :data-target="'#changeUserModal'+user_id">
    Редактировать
  </button>
  <teleport to="body">
    <div class="modal fade" :id="'changeUserModal'+user_id" tabindex="-1" role="dialog"
         :aria-labelledby="'changeUserModalTitle'+user_id"
         aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" :id="'changeUserModalTitle'+user_id">Изменить</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <user-form-component :user_id="user_id"></user-form-component>
          </div>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script>
import {mapState, mapActions, mapGetters, mapMutations} from 'vuex'
import UserFormComponent from "./UserFormComponent";

export default {
  name: "UserChangeComponent",
  components: {UserFormComponent},
  props: {
    user_id: {
      type: String,
      required: true
    },
  },
  methods: {},

}
</script>

<style lang="scss" scoped>

</style>