import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        current_user_checkers: [],
        available_for_select_checkers: [],
    }),
    mutations: {
        setCurrentUserCheckers(state, val) {
            state.current_user_checkers = (val || [])
        },
        setAvailableCheckers(state, val) {
            state.available_for_select_checkers = (val || [])
        },
    },
    getters: {
        availableCheckers(state, getters) {
            return state.available_for_select_checkers
        },
        currentUserCheckers(state, getters) {
            return state.current_user_checkers
        },
        currentUserPendingCheckers(state, getters) {
            return _.filter(getters.currentUserCheckers, (el) => Number(el.approvalStatus) === 0)
        },
        currentUserRejectedCheckers(state, getters) {
            return _.filter(getters.currentUserCheckers, (el) => Number(el.approvalStatus) === 1)
        },
        currentUserApprovedCheckers(state, getters) {
            return _.filter(getters.currentUserCheckers, (el) => Number(el.approvalStatus) === 2)
        },
    },
    actions: {
        async fetchCurrentUserCheckers({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.current_user_checkers && state.current_user_checkers.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/checkers/get-user-checkers/${rootGetters.currentUser.id}`)
                commit('setCurrentUserCheckers', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchAvailableCheckers({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.available_for_select_checkers && state.available_for_select_checkers.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/checkers/get-available-checkers/${rootGetters.currentUser.id}`)
                commit('setAvailableCheckers', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getChecker({commit, state, dispatch, getters, rootGetters}, checker_id) {
            if (!checker_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/checkers/constructed/${checker_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
        async deleteChecker({commit, state, dispatch, getters, rootGetters}, checker_id) {
            try {
                let {data} = await axios.get(`/api/checkers/delete-checker/${checker_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}