import api from '../../services/api-configurator'
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
        async getSolution({commit, state, dispatch, getters, rootGetters}, solution_id) {
            if (!solution_id) {
                return null
            }
            try {
                let {data} = await rootGetters.api.get(`/solutions/${solution_id}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async fetchAvailableCompilers({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.available_compilers && state.available_compilers.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/solutions/compilers')
                commit('setAvailableCompilers', data)
            } catch (e) {
                console.error(e)
            }
        },
        async sendSolution({commit, state, dispatch, getters, rootGetters}, request) {
            try {
                let {data} = await rootGetters.api.post(`/solutions`, request)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async compileSolution({commit, state, dispatch, getters, rootGetters}, solution_id) {
            try {
                let {status, errors} = await rootGetters.api.post(`/solutions/${solution_id}/compile`)
                return status
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async runSolutionTests({commit, state, dispatch, getters, rootGetters}, solution_id) {
            try {
                let {data} = await rootGetters.api.post(`/solutions/${solution_id}/run`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async changeSolution({commit, state, dispatch, getters, rootGetters}, {
            contestId,
            solutionId,
            verdict,
            points
        }) {
            try {
                let {data} = await rootGetters.api.put(`/contests/management/${contestId}/solutions/${solutionId}`, {
                    solutionId,
                    verdict,
                    points
                })
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async deleteSolution({commit, state, dispatch, getters, rootGetters}, {
            contestId,
            solutionId,
        }) {
            try {
                let {data} = await rootGetters.api.delete(`/contests/management/${contestId}/solutions/${solutionId}`)
                if (data && data.status) {
                    commit(
                        'setCurrentContestSolutionsForCurrentUser',
                        _.filter(
                            rootGetters.currentContestSolutionsForCurrentUser,
                            (s) => +s.id !== +solutionId
                        )
                    )
                    commit(
                        'setCurrentContestAllSolutions',
                        _.filter(
                            rootGetters.currentContestAllSolutions,
                            (s) => +s.id !== +solutionId
                        )
                    )
                }
                return data.status
            } catch (e) {
                console.error(e)
                return false
            }
        },

        /**
         *
         * @param update_callback получит массив {index, props, solution_user_id}
         * @param current_solutions_collection
         * @param solution_data объект похожий по структуре на решение(есть actualResult)
         * @param is_solution_data_full передан ли нормальный объект решения в solution_data
         * @returns {Promise<void>}
         */
        async updateOrAddSolutionToState({commit, state, dispatch, getters, rootGetters}, {
            current_solutions_collection,
            solution_data,
            is_solution_data_full,
            update_callback,
        }) {
            let solutionFromDB, solution_user_id;
            let props = solution_data
            let index = _.findIndex(current_solutions_collection, (s) => +s.id === +solution_data.actualResult.solutionId)
            if (+index > -1) {
                solution_user_id = current_solutions_collection[index].participant.id
            } else {
                if (is_solution_data_full) {
                    solutionFromDB = solution_data
                } else {
                    solutionFromDB = await dispatch('getSolution', +solution_data.actualResult.solutionId)
                }
                if (solutionFromDB) {
                    index = current_solutions_collection.length;
                    props = solutionFromDB;
                    solution_user_id = solutionFromDB.participant.id
                }
            }
            return update_callback({index, props, solution_user_id})
        }
    }
}