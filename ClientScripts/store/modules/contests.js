import api from '../../services/api-configurator'
import moment from 'moment'
import 'moment-timezone';
import * as _ from 'lodash'

export default {
    state: () => ({
        current_contest: null,
        current_contest_participants: [],
        current_contest_monitor_entries: [],
        current_contest_user_stats: null,
        current_contest_rules_set: null,

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
        setCurrentContestRulesSet(state, val) {
            state.current_contest_rules_set = val
        },
        setCurrentContestParticipants(state, val) {
            state.current_contest_participants = (val || [])
        },
        setCurrentContestMonitor(state, val) {
            state.current_contest_monitor_entries = (val || [])
        },
        setCurrentContestUserStats(state, val) {
            state.current_contest_user_stats = val
        }
    },
    getters: {
        currentContest(state, getters) {
            return state.current_contest
        },
        currentContestRulesSet(state, getters) {
            return state.current_contest_rules_set
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
        currentContestUserStats(state, getters) {
            return state.current_contest_user_stats
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
            return moment.utc(getters.currentContest.endDateTimeUTC).isSameOrBefore(moment.utc())
        },
        currentContestIsInTheFuture(state, getters) {
            if (!getters.currentContest) {
                return true
            }
            return moment.utc(getters.currentContest.startDateTimeUTC).isAfter(moment.utc())
        },
        currentUserIsParticipantOfCurrentContest(state, getters) {
            if (!getters.currentUser || !getters.currentContest) {
                return false
            }
            return !!_.find(getters.currentContestParticipants, (p) => +p.userId === +getters.currentUser.id)
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
            let contest = await dispatch('getLocalizedContest', contest_id)
            let rulesSet = await dispatch('getContestRulesSet', contest_id)
            let participants = await dispatch('getContestParticipants', contest_id)
            let monitor = await dispatch('getContestMonitor', contest_id)

            commit('setCurrentContest', contest)
            commit('setCurrentContestRulesSet', rulesSet)
            commit('setCurrentContestParticipants', participants)
            commit('setCurrentContestMonitor', monitor)

            let currentUserParticipant = _.find(participants, (p) => +p.userId === +rootGetters.currentUser?.id)
            if (currentUserParticipant) {
                let solutions = await dispatch('getUserSolutionsInContest', {
                    contest_id,
                    user_id: rootGetters.currentUser?.id
                })
                let stats = await dispatch('getUserStatsInContest', {
                    contest_id,
                    user_id: rootGetters.currentUser?.id
                })
                commit('setCurrentContestSolutionsForCurrentUser', solutions)
                commit('setCurrentContestUserStats', stats)
            }
        },
        async getContestParticipants({commit, state, dispatch, getters, rootGetters}, contest_id) {
            if (!contest_id) {
                return []
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/participants/${contest_id}`)
                return data
            } catch (e) {
                console.error(e)
                return []
            }
        },
        async getContestMonitor({commit, state, dispatch, getters, rootGetters}, contest_id) {
            if (!contest_id) {
                return []
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/${contest_id}/monitor`)
                return data
            } catch (e) {
                console.error(e)
                return []
            }
        },
        async getContestRulesSet({ commit, state, dispatch, getters, rootGetters }, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/${contest_id}/rules`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async getUserStatsInContest({ commit, state, dispatch, getters, rootGetters }, { contest_id, user_id }) {
            if (!contest_id || !user_id) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/participants/${contest_id}/stats/${user_id}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async getContestProblem({ commit, state, dispatch, getters, rootGetters }, { contest_id, letter }) {
            if (!contest_id || !letter) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/${contest_id}/problems/${letter}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async getUserSolutionsInContest({commit, state, dispatch, getters, rootGetters}, {contest_id, user_id}) {
            if (!contest_id || !user_id) {
                return []
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/${contest_id}/solutions/${user_id}`)
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
                let {data} = await rootGetters.api.get('/contests/running/ru')
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
                let {data} = await rootGetters.api.get('/contests/upcoming/ru')
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
                let {data} = await rootGetters.api.get('/contests/participating/ru')
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
                let {data} = await rootGetters.api.get(`/workspace/contests/user/${rootGetters.currentUser.id}/ru`)
                commit('setCurrentUserContests', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getLocalizedContest({commit, state, dispatch, getters, rootGetters}, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/contests/${contest_id}/ru`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },  
        async getWorkspaceContest({commit, state, dispatch, getters, rootGetters}, contest_id) {
            if (!contest_id) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/workspace/contests/${contest_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
        async addUserToContest({commit, state, dispatch, getters, rootGetters}, {user_name, user_id, contest_id}) {
            try {
                let {data} = await rootGetters.api.post(`/contests/participants/${contest_id}`, {
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
        async removeUserFromContest({commit, state, dispatch, getters, rootGetters}, {user_id, contest_id}) {
            if (!user_id || !contest_id) {
                return {}
            }
            try {
                let {data} = await rootGetters.api.delete(`/contests/participants/${contest_id}/${user_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
        async deleteContest({commit, state, dispatch, getters, rootGetters}, contest_id) {
            try {
                let {data} = await rootGetters.api.delete(`/workspace/contests/${contest_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}

        }
    }
}