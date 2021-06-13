﻿import axios from 'axios'
import * as _ from 'lodash'
import TestResultVerdicts from "../../dictionaries/TestResultVerdicts";

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
    },
    actions: {
        getVerdictName({commit, state, dispatch, getters}, verdict) {
            return _.find(_.toPairs(TestResultVerdicts), (p) => +p[1] === +verdict)[0]
        },

        getLastTestNumber({commit, state, dispatch, getters}, solution) {
            return _.maxBy((solution?.testResults || []), (t) => +t.number)?.number
        },
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
        async compileSolution({commit, state, dispatch, getters}, request) {
            try {
                let {data} = await axios.post(`/api/solutions/compile-solution`, request)
                return data
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