import * as _ from "lodash";

export default {
    namespaced: true,
    state: () => ({
        moderating_ruleset: null,
        rulesets_to_moderate: [],
        approved_rulesets: [],
        rejected_rulesets: [],
    }),
    mutations: {
        setCurrentModeratingRuleSet(state, val) {
            state.moderating_ruleset = val
        },
        setRuleSetsToModerate(state, val) {
            state.rulesets_to_moderate = (val || [])
        },
        setApprovedRuleSet(state, val) {
            state.approved_rulesets = (val || [])
        },
        setRejectedRuleSet(state, val) {
            state.rejected_rulesets = (val || [])
        },
    },
    getters: {
        currentModeratingRuleSet(state, getters) {
            return state.moderating_ruleset
        },
        ruleSetsToModerate(state, getters) {
            return state.rulesets_to_moderate
        },
        approvedRuleSet(state, getters) {
            return state.approved_rulesets
        },
        rejectedRuleSet(state, getters) {
            return state.rejected_rulesets
        },
        allRuleSet(state, getters) {
            return _.concat(getters.ruleSetsToModerate, getters.approvedRuleSet, getters.rejectedRuleSet)
        }
    },
    actions: {
        async changeCurrentRuleSet({commit, state, dispatch, getters}, {force, rule_set_id}) {
            await dispatch('fetchRuleSetsToModerate', force)
            await dispatch('fetchApprovedRuleSet', force)
            await dispatch('fetchRejectedRuleSet', force)
            if (!force && +getters.currentModeratingRuleSet?.id === +rule_set_id) {
                return
            }
            let rule_set = await dispatch('getWorkspaceRulesSet', rule_set_id, { root: true })
            commit('setCurrentModeratingRuleSet', rule_set)
        },
        async fetchRuleSetsToModerate({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.rulesets_to_moderate && state.rulesets_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/workspace/rules/requests')
                commit('setRuleSetsToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedRuleSet({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.approved_rulesets && state.approved_rulesets.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/workspace/rules/accepted')
                commit('setApprovedRuleSet', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedRuleSet({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.rejected_rulesets && state.rejected_rulesets.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/workspace/rules/rejected')
                commit('setRejectedRuleSet', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderateRuleSet({commit, state, dispatch, getters, rootGetters}, {rule_set_id, request_body}) {
            try {
                let {data} = await rootGetters.api.put(`/workspace/rules/${rule_set_id}/moderate`, request_body)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}