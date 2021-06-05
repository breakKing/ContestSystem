import axios from 'axios'
import * as _ from 'lodash'
export default {
    state: () => ({
        currently_running_contests: [],
        available_for_user_contests: [],
        participating_contests: [],
        current_user_contests: [],
    }),
    mutations: {
        setRunningContests(state, val) {
            state.currently_running_contests = (val || [])
        },
        setAvailableContests(state, val) {
            state.available_for_user_contests = (val || [])
        },
        setParticipatingContests(state, val) {
            state.participating_contests = (val || [])
        },
        setCurrentUserContests(state, val) {
            state.current_user_contests = (val || [])
        },
    },
    getters: {
        runningContests(state, getters) {
            return state.currently_running_contests
        },
        availableContests(state, getters) {
            return state.available_for_user_contests
        },
        participatingContests(state, getters) {
            return state.participating_contests
        },
        currentUserContests(state, getters) {
            return state.current_user_contests
        },
        currentUserPendingContests(state, getters) {
            return _.filter(getters.currentUserContests, (el) => Number(el.approvalStatus) === 0)
        },
        currentUserRejectedContests(state, getters) {
            return _.filter(getters.currentUserContests, (el) => Number(el.approvalStatus) === 1)
        },
        currentUserApprovedContests(state, getters) {
            return _.filter(getters.currentUserContests, (el) => Number(el.approvalStatus) === 2)
        },
    },
    actions: {
        async fetchRunningContests({commit, state, dispatch, getters}, force = false) {
            if (!force && state.currently_running_contests && state.currently_running_contests.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/contests/get-running-contests/ru')
                commit('setRunningContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchAvailableContests({commit, state, dispatch, getters}, force = false) {
            if (!force && state.available_for_user_contests && state.available_for_user_contests.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/contests/get-available-contests/ru')
                commit('setAvailableContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchParticipatingContests({commit, state, dispatch, getters}, force = false) {
            if (!force && state.participating_contests && state.participating_contests.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/contests/get-participating-contests/ru')
                commit('setParticipatingContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchCurrentUserContestsList({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.current_user_contests_list && state.current_user_contests_list.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/contests/get-user-created-contests/${rootGetters.currentUser.id}/ru`)
                commit('setCurrentUserContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getContestById({commit, state, dispatch, getters}, contest_id) {
            await dispatch('fetchAvailableContests');
            let local = _.find(getters.availableContests, (c) => +c.id === +contest_id)
            if (!local) {
                local = await dispatch('getPublishedContest', contest_id)
            }
            return local
        },
        async getPublishedContest({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/ru`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
    }
}