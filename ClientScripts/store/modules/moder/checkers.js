import axios from 'axios'
import ApproveTypes from "../../../dictionaries/ApproveTypes";
import * as _ from "lodash";

export default {
    namespaced: true,
    state: () => ({
        moderating_checker: null,
        checkers_to_moderate: [],
        approved_checkers: [],
        rejected_checkers: [],
    }),
    mutations: {
        setCurrentModeratingChecker(state, val) {
            state.moderating_checker = val
        },
        setCheckersToModerate(state, val) {
            state.checkers_to_moderate = (val || [])
        },
        setApprovedCheckers(state, val) {
            state.approved_checkers = (val || [])
        },
        setRejectedCheckers(state, val) {
            state.rejected_checkers = (val || [])
        },
    },
    getters: {
        currentModeratingChecker(state, getters) {
            return state.moderating_checker
        },
        checkersToModerate(state, getters) {
            return state.checkers_to_moderate
        },
        approvedCheckers(state, getters) {
            return state.approved_checkers
        },
        rejectedCheckers(state, getters) {
            return state.rejected_checkers
        },
        allCheckers(state, getters) {
            return _.concat(getters.checkersToModerate, getters.approvedCheckers, getters.rejectedCheckers)
        }
    },
    actions: {
        changeCurrentChecker({commit, state, dispatch, getters}, {force, checker_id}) {
            if (!force && +getters.currentModeratingChecker?.id === +checker_id) {
                return
            }
            commit('setCurrentModeratingChecker', _.find(getters.allCheckers, (el) => +el.id === +checker_id))
        },
        async fetchCheckersToModerate({commit, state, dispatch, getters}, force = false) {
            if (!force && state.checkers_to_moderate && state.checkers_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/checkers/get-requests')
                commit('setCheckersToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedCheckers({commit, state, dispatch, getters}, force = false) {
            if (!force && state.approved_checkers && state.approved_checkers.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/checkers/get-approved')
                commit('setApprovedCheckers', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedCheckers({commit, state, dispatch, getters}, force = false) {
            if (!force && state.rejected_checkers && state.rejected_checkers.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/checkers/get-rejected')
                commit('setRejectedCheckers', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderateChecker({commit, state, dispatch, getters}, {checker_id, request_body}) {
            try {
                let {data} = await axios.put(`/api/checkers/moderate/${checker_id}`, request_body)
                commit('setCurrentModeratingChecker', data)
            } catch (e) {
                console.error(e)
            }
        },
    }
}