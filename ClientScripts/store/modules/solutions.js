import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        // выбранное для просмотра
        current_solution: null,
        available_compilers: [],
    }),
    mutations: {
        setCurrentSolution(state, val) {
            state.current_solution = val
        },
        setAvailableCompilers(state, val) {
            state.available_compilers = (val || [])
        },
    },
    getters: {
        availableCompilers(state, getters) {
            return state.available_compilers
        },
        currentSolution(state, getters) {
            return state.current_solution
        },
    },
    actions: {
        async getSolution({commit, state, dispatch, getters}, solution_id) {
            if (!solution_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/solutions/constructed/${solution_id}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async getUserSolutionsInContest({commit, state, dispatch, getters}, {contest_id, user_id}) {
            if (!contest_id || !user_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/contests/${contest_id}/get-solutions/${user_id}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async fetchAvailableCompilers({commit, state, dispatch, getters}, force = false) {
            if (!force && state.available_compilers && state.available_compilers.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/solutions/get-compilers')
                commit('setAvailableCompilers', data)
            } catch (e) {
                console.error(e)
            }
        },
        async compileSolution({commit, state, dispatch, getters}, request) {
            try {
                let {data} = await axios.post(`/api/solutions/compile-solution`, request)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async runSolutionTest({commit, state, dispatch, getters}, {solution_id, test_number}) {
            try {
                let {data} = await axios.post(`/api/solutions/${solution_id}/run-test/${test_number}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
    }
}