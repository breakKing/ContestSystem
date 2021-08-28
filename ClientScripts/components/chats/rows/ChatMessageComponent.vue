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
      return +this.currentUser?.id === +this.message?.initiator?.id
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

.message-up-block {
  margin-bottom: 0.625rem;
}

.message {
  position: relative;
  background-color: #BFE8AF;
  border-radius: 0.625rem;
  padding: 0.625rem;

  &:after {
    bottom: 100%;
    left: 5%;
    content: " ";
    height: 0;
    width: 0;
    position: absolute;
    pointer-events: none;
    border: 0.625rem solid transparent;
    border-bottom-color: #BFE8AF;
    margin-left: -0.625rem;
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