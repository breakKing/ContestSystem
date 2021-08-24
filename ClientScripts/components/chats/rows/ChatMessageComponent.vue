<template>
  <div class="row-wrapper d-flex mb-3" :class="{'justify-content-end': isOwn}">
    <div class="message-wrapper">
      <div class="message-up-block">{{ getFormattedFullDateTime(message.dateTimeUTC) }} {{
          isOwn ? 'Вы' : user_alias
        }}
      </div>
      <div class="message" :class="{'my-own-message': isOwn}">{{ message.text }}</div>
    </div>
  </div>
</template>

<script>
import {mapGetters} from "vuex";

export default {
  name: "ChatMessageComponent",
  props: {
    message: Object,
    chat_users: Array,
  },
  computed: {
    ...mapGetters(['currentUser', 'getFormattedFullDateTime']),
    isOwn() {
      return +this.currentUser?.id === +this.message?.initiator?.userId
    },
    user_alias() {
      return this.message?.initiator?.name
    }
  }
}
</script>

<style lang="scss" scoped>
.message-wrapper {
  width: 60%;
}

.message {
  position: relative;
  background-color: #86BB71;
  border-radius: 10px;
  padding: 10px;

  &:after {
    bottom: 100%;
    left: 5%;
    content: " ";
    height: 0;
    width: 0;
    position: absolute;
    pointer-events: none;
    border: 10px solid transparent;
    border-bottom-color: #86BB71;
    margin-left: -10px;
  }
}

.my-own-message {
  background-color: #94C2ED;

  &:after {
    border-bottom-color: #94C2ED;
    left: 95%;
  }
}
</style>