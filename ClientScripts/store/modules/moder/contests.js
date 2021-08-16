import axios from 'axios'
import ApproveTypes from "../../../dictionaries/ApproveTypes";
import * as _ from "lodash";

export default {
    namespaced: true,
    state: () => ({
        moderating_contest: null,
        contests_to_moderate: [],
        approved_contests: [],
        rejected_contests: [],
    }),
    mutations: {
        setCurrentModeratingContest(state, val) {
            state.moderating_contest = val
        },
        setContestsToModerate(state, val) {
            state.contests_to_moderate = (val || [])
        },
        setApprovedContests(state, val) {
            state.approved_contests = (val || [])
        },
        setRejectedContests(state, val) {
            state.rejected_contests = (val || [])
        },
    },
    getters: {
        currentModeratingContest(state, getters) {
            return state.moderating_contest
        },
        currentModeratingContestLocalizer(state, getters) {
            return _.find((getters.currentModeratingContest?.localizers || []), (l) => l.culture === 'ru')
        },
        contestsToModerate(state, getters) {
            return state.contests_to_moderate
        },
        approvedContests(state, getters) {
            return state.approved_contests
        },
        rejectedContests(state, getters) {
            return state.rejected_contests
        },
        allContests(state, getters) {
            return _.concat(getters.contestsToModerate, getters.approvedContests, getters.rejectedContests)
        }
    },
    actions: {
        async changeCurrentContest({commit, state, dispatch, getters}, {force, contest_id}) {
            await dispatch('fetchContestsToModerate', force)
            await dispatch('fetchApprovedContests', force)
            await dispatch('fetchRejectedContests', force)
            if (!force && +getters.currentModeratingContest?.id === +contest_id) {
                return
            }
            let contest = await dispatch('getContestById', contest_id, { root: true })
            commit('setCurrentModeratingContest', contest)
        },
        async fetchContestsToModerate({commit, state, dispatch, getters}, force = false) {
            if (!force && state.contests_to_moderate && state.contests_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/workspace/contests/requests')
                commit('setContestsToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedContests({commit, state, dispatch, getters}, force = false) {
            if (!force && state.approved_contests && state.approved_contests.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/workspace/contests/accepted')
                commit('setApprovedContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedContests({commit, state, dispatch, getters}, force = false) {
            if (!force && state.rejected_contests && state.rejected_contests.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/workspace/contests/rejected')
                commit('setRejectedContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderateContest({commit, state, dispatch, getters}, {contest_id, request_body}) {
            try {
                let {data} = await axios.put(`/api/workspace/contests/${contest_id}/moderate`, request_body)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}