import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        available_for_user_rulesets: [],
        current_user_rulesets: [],
    }),
    mutations: {
        setAvailableRuleSets(state, val) {
            state.available_for_user_rulesets = (val || [])
        },
        setCurrentUserRuleSets(state, val) {
            state.current_user_rulesets = (val || [])
        },
    },
    getters: {
        availableRuleSets(state, getters) {
            return state.available_for_user_rulesets
        },
        currentUserRuleSets(state, getters) {
            return state.current_user_rulesets
        },
    },
    actions: {
        async fetchAvailableRuleSets({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.available_for_user_rulesets && state.available_for_user_rulesets.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/workspace/rules/available/${rootGetters.currentUser.id}`)
                commit('setAvailableRuleSets', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchCurrentUserRuleSets({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.current_user_rulesets && state.current_user_rulesets.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/workspace/rules/user/${rootGetters.currentUser.id}`)
                commit('setCurrentUserRuleSets', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getRuleSet({getters, dispatch}, set_id) {
            if (!set_id) {
                return null
            }
            await dispatch('fetchAvailableRuleSets')
            return _.find(getters.availableRuleSets, (rs) => +rs.id === +set_id)
        },
        async deleteRuleSet({getters, dispatch}, ruleset_id) {
            try {
                let {data} = await axios.delete(`/api/workspace/rules/${ruleset_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        }
    }
}