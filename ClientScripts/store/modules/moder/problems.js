import axios from 'axios'
import ApproveTypes from "../../../dictionaries/ApproveTypes";
import * as _ from "lodash";

export default {
    namespaced: true,
    state: () => ({
        moderating_problem: null,
        problems_to_moderate: [],
        approved_problems: [],
        rejected_problems: [],
    }),
    mutations: {
        setCurrentModeratingProblem(state, val) {
            state.moderating_problem = val
        },
        setProblemsToModerate(state, val) {
            state.problems_to_moderate = (val || [])
        },
        setApprovedProblems(state, val) {
            state.approved_problems = (val || [])
        },
        setRejectedProblems(state, val) {
            state.rejected_problems = (val || [])
        },
    },
    getters: {
        currentModeratingProblem(state, getters) {
            return state.moderating_problem
        },
        problemsToModerate(state, getters) {
            return state.problems_to_moderate
        },
        approvedProblems(state, getters) {
            return state.approved_problems
        },
        rejectedProblems(state, getters) {
            return state.rejected_problems
        },
        allProblems(state, getters) {
            return _.concat(getters.problemsToModerate, getters.approvedProblems, getters.rejectedProblems)
        }
    },
    actions: {
        changeCurrentProblem({commit, state, dispatch, getters}, {force, problem_id}) {
            if (!force && +getters.currentModeratingProblem?.id === +problem_id) {
                return
            }
            commit('setCurrentModeratingProblem', _.find(getters.allProblems, (el) => +el.id === +problem_id))
        },
        async fetchProblemsToModerate({commit, state, dispatch, getters}, force = false) {
            if (!force && state.problems_to_moderate && state.problems_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/problems/get-requests')
                commit('setProblemsToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedProblems({commit, state, dispatch, getters}, force = false) {
            if (!force && state.approved_problems && state.approved_problems.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/problems/get-approved')
                commit('setApprovedProblems', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedProblems({commit, state, dispatch, getters}, force = false) {
            if (!force && state.rejected_problems && state.rejected_problems.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/problems/get-rejected')
                commit('setRejectedProblems', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderateProblem({commit, state, dispatch, getters}, {problem_id, request_body}) {
            try {
                let {data} = await axios.put(`/api/problems/moderate/${problem_id}`, request_body)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}