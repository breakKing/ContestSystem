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
        currentUserPendingRuleSets(state, getters) {
            return _.filter(getters.currentUserRuleSets, (el) => Number(el.approvalStatus) === 0)
        },
        currentUserRejectedRuleSets(state, getters) {
            return _.filter(getters.currentUserRuleSets, (el) => Number(el.approvalStatus) === 1)
        },
        currentUserApprovedRuleSets(state, getters) {
            return _.filter(getters.currentUserRuleSets, (el) => Number(el.approvalStatus) === 2)
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
                let {data} = await rootGetters.api.get(`/workspace/rules/available/${rootGetters.currentUser.id}`)
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
                let {data} = await rootGetters.api.get(`/workspace/rules/user/${rootGetters.currentUser.id}`)
                commit('setCurrentUserRuleSets', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getWorkspaceRulesSet({getters, dispatch, rootGetters}, ruleset_id) {
            if (!ruleset_id) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/workspace/rules/${ruleset_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
        async deleteRuleSet({getters, dispatch, rootGetters}, ruleset_id) {
            try {
                let {data} = await rootGetters.api.delete(`/workspace/rules/${ruleset_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        }
    }
}