import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        current_contest: null,
        current_contest_participants: [],
        current_contest_monitor_entries: [],

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

        setCurrentContest(state, val) {
            state.current_contest = val
        },
        setCurrentContestParticipants(state, val) {
            state.current_contest_participants = (val || [])
        },
        setCurrentContestMonitor(state, val) {
            state.current_contest_monitor_entries = (val || [])
        },
    },
    getters: {
        currentContest(state, getters) {
            return state.current_contest
        },
        currentContestParticipants(state, getters) {
            return state.current_contest_participants
        },
        currentContestMonitorEntries(state, getters) {
            return state.current_contest_monitor_entries
        },

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
        // контест и все связанные данные
        async changeCurrentContest({commit, state, dispatch, getters}, contest_id) {
            let contest = await dispatch('getContestById', contest_id)
            let participants = await dispatch('getContestParticipants', contest_id)
            let monitor = await dispatch('getContestMonitor', contest_id)

            commit('setCurrentContest', contest)
            commit('setCurrentContestParticipants', participants)
            commit('setCurrentContestMonitor', monitor)
        },
        async getContestById({commit, state, dispatch, getters}, contest_id) {
            await dispatch('fetchAvailableContests');
            if (!contest_id) {
                return null;
            }
            let local = _.find(getters.availableContests, (c) => +c.id === +contest_id)
            if (!local) {
                local = await dispatch('getPublishedContest', contest_id)
            }
            return local
        },
        async getContestParticipants({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/get-participants`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async getContestMonitor({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/get-monitor`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },

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
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.current_user_contests && state.current_user_contests.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/contests/get-user-created-contests/${rootGetters.currentUser.id}/ru`)
                commit('setCurrentUserContests', data)
            } catch (e) {
                console.error(e)
            }
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
        async addUserToContest({commit, state, dispatch, getters}, {user_name, user_id, contest_id}) {
            try {
                let {data} = await axios.post(`/api/contests/${contest_id}/add-participant`,{
                    // псевдоним и user_id
                })
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
        async removeUserFromContest({commit, state, dispatch, getters}, {user_id, contest_id}) {
            try {
                let {data} = await axios.post(`/api/contests/${contest_id}/delete-participant/${user_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
    }
}