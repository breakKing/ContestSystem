import axios from 'axios'
import moment from 'moment'
import 'moment-timezone';
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
        current_contest_solutions_for_current_user: [],
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
        setCurrentContestSolutionsForCurrentUser(state, val) {
            state.current_contest_solutions_for_current_user = (val || [])
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
        currentContestSolutionsForCurrentUser(state, getters) {
            return state.current_contest_solutions_for_current_user
        },
        currentContestMonitorEntries(state, getters) {
            return _.sortBy((state.current_contest_monitor_entries || []), ['position'])
        },
        currentUserIsOwnerOfCurrentContest(state, getters) {
            if (!getters.currentUser || !getters.currentContest?.creator) {
                return false
            }
            return +getters.currentContest.creator.id === +getters.currentUser.id
        },
        currentContestIsRunning(state, getters) {
            if (!getters.currentContest) {
                return false
            }
            return !getters.currentContestIsInPast && !getters.currentContestIsInTheFuture
        },
        currentContestIsInPast(state, getters) {
            if (!getters.currentContest) {
                return true
            }
            return moment(getters.currentContest.endDateTimeUTC).tz('Europe/Moscow').isSameOrBefore(moment().tz('Europe/Moscow'))
        },
        currentContestIsInTheFuture(state, getters) {
            if (!getters.currentContest) {
                return true
            }
            return moment(getters.currentContest.startDateTimeUTC).tz('Europe/Moscow').isAfter(moment().tz('Europe/Moscow'))
        },
        currentUserIsParticipantOfCurrentContest(state, getters) {
            if (!getters.currentUser || !getters.currentContest) {
                return false
            }
            return !!_.find(getters.currentContestParticipants, (p) => +p.id === +getters.currentUser.id)
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
        async changeCurrentContest({commit, state, dispatch, getters, rootGetters}, {force, contest_id}) {
            if (!force && +getters.currentContest?.id === +contest_id) {
                return
            }
            let contest = await dispatch('getContestById', contest_id)
            let participants = await dispatch('getContestParticipants', contest_id)
            let monitor = await dispatch('getContestMonitor', contest_id)
            let solutions = await dispatch('getUserSolutionsInContest', {
                contest_id,
                user_id: rootGetters.currentUser?.id
            })

            commit('setCurrentContest', contest)
            commit('setCurrentContestParticipants', participants)
            commit('setCurrentContestMonitor', monitor)
            commit('setCurrentContestSolutionsForCurrentUser', solutions)
        },
        async getContestById({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return null;
            }
            return await dispatch('getConstructedContest', contest_id)
        },
        async getContestParticipants({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return []
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/get-participants`)
                return data
            } catch (e) {
                console.error(e)
                return []
            }
        },
        async getContestMonitor({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return []
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/get-monitor`)
                return data
            } catch (e) {
                console.error(e)
                return []
            }
        },
        async getUserSolutionsInContest({commit, state, dispatch, getters}, {contest_id, user_id}) {
            if (!contest_id || !user_id) {
                return []
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/get-solutions/${user_id}`)
                return data
            } catch (e) {
                console.error(e)
                return []
            }
        },
        async fetchRunningContests({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.currently_running_contests && state.currently_running_contests.length > 0) {
                return
            }
            if (!rootGetters.currentUser){
                return
            }
            try {
                let {data} = await axios.get('/api/contests/get-running-contests/ru')
                commit('setRunningContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchAvailableContests({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.available_for_user_contests && state.available_for_user_contests.length > 0) {
                return
            }
            if (!rootGetters.currentUser){
                return
            }
            try {
                let {data} = await axios.get('/api/contests/get-available-contests/ru')
                commit('setAvailableContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchParticipatingContests({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.participating_contests && state.participating_contests.length > 0) {
                return
            }
            if (!rootGetters.currentUser){
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
        async getConstructedContest({commit, state, dispatch, getters}, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/contests/constructed/${contest_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
        async addUserToContest({commit, state, dispatch, getters}, {user_name, user_id, contest_id}) {
            try {
                let {data} = await axios.post(`/api/contests/${contest_id}/add-participant`, {
                    userId: user_id,
                    contestId: contest_id,
                    alias: user_name,
                })
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
        async removeUserFromContest({commit, state, dispatch, getters}, {user_id, contest_id}) {
            if (!user_id || !contest_id) {
                return {}
            }
            try {
                let {data} = await axios.delete(`/api/contests/${contest_id}/delete-participant/${user_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
        async deleteContest({commit, state, dispatch, getters}, contest_id) {
            try {
                let {data} = await axios.delete(`/api/contests/delete-contest/${contest_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}

        }
    }
}