import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        available_compilers: [],
    }),
    mutations: {
        setAvailableCompilers(state, val) {
            state.available_compilers = (val || [])
        },
    },
    getters: {
        availableCompilers(state, getters) {
            return state.available_compilers
        },
        getLastTestNumber: (state, getters) => (solution) => {
            if (solution && solution.actualResult && solution.actualResult.lastRunTestNumber) {
                return +solution.actualResult.lastRunTestNumber
            }
            return +_.maxBy((solution?.testResults || []), (t) => +t.number)?.number
        },
    },
    actions: {
        async getSolution({commit, state, dispatch, getters}, solution_id) {
            if (!solution_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/solutions/${solution_id}`)
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
        async sendSolution({commit, state, dispatch, getters}, request) {
            try {
                let {data} = await axios.post(`/api/solutions/send-solution`, request)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async compileSolution({commit, state, dispatch, getters}, solution_id) {
            try {
                let {status, errors} = await axios.post(`/api/solutions/${solution_id}/compile`)
                return status
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async runSolutionTests({commit, state, dispatch, getters}, solution_id) {
            try {
                let {data} = await axios.post(`/api/solutions/${solution_id}/run-tests`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
    }
}