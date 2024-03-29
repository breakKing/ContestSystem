﻿import * as _ from 'lodash'
import $ from "jquery";
import moment from "moment";

export default {
    state: () => ({
        current_user_chats: [],
    }),
    mutations: {
        setCurrentUserChats(state, val) {
            state.current_user_chats = (val || [])
        },
        updateCurrentUserChats(state, chat_data) {
            let chat = _.find(state.current_user_chats, (c) => +c.id === +chat_data.id)
            if (chat) {
                chat_data.historyEntries = _.uniqBy(_.concat((chat_data.historyEntries || []), (chat.historyEntries || [])), (c) => c.id)
                Object.assign(chat, chat_data)
            } else {
                state.current_user_chats = _.concat(state.current_user_chats, [chat_data])
            }
        },
        updateHistoryEntriesInChat(state, {chat_id, history_entry}) {
            if (state.current_user_chats) {
                let chat = _.find(state.current_user_chats, (c) => +c.id === +chat_id)
                if (chat) {
                    Object.assign(chat, {historyEntries: _.concat((chat.historyEntries || []), [history_entry])})
                }
            }
        },
    },
    getters: {
        currentUserChats(state, getters) {
            return state.current_user_chats
        },
        getContestChats: (state, getters) => (contest_id) => {
            return _.filter(state.current_user_chats, (c) => +c.contestId === +contest_id)
        },
    },
    actions: {
        async fetchChatInfo({commit, state, dispatch, getters, rootGetters}, {link, offset}) {
            try {
                let {data} = await rootGetters.api.get(`/messenger/chats/${link}?offset=${offset}`)
                commit('updateCurrentUserChats', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchUserChatsFromContest({commit, state, dispatch, getters, rootGetters}, {contest_id}) {
            if (!contest_id) {
                return
            }
            try {
                let chats = await dispatch('getChatsFromContest', {contest_id})
                for (let chat of chats) {
                    commit('updateCurrentUserChats', chat)
                }
            } catch (e) {
                console.error(e)
            }
        },
        async getChatsFromContest({commit, state, dispatch, getters, rootGetters}, {contest_id}) {
            if (!contest_id) {
                return []
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/${contest_id}/chats`)
                return data
            } catch (e) {
                console.error(e)
                return []
            }
        },
        async sendMessageToChat({commit, state, dispatch, getters, rootGetters}, {chat, text}) {
            try {
                if (!rootGetters.currentUser) {
                    return false
                }
                if (chat) {
                    let {data} = await rootGetters.api
                        .post(`/messenger/chats/${chat.link}/messages`, {
                            chatLink: chat.link,
                            userId: rootGetters.currentUser.id,
                            text
                        })
                    return true
                }
                return false
            } catch (e) {
                console.error(e)
                return false
            }
        },
    }
}