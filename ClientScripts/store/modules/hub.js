import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr"
import * as _ from "lodash"

export default {
    state: () => ({
        hub_connection: null,
    }),
    mutations: {
        setHubConnection(state, val) {
            state.hub_connection = val
        },
    },
    getters: {
        hubConnection(state, getters) {
            return state.hub_connection
        },
    },
    actions: {
        async initHub({ commit, state, dispatch, getters, rootGetters }, token) {
            // TODO: понять, почему не работает WebSockets (из-за этого ошибка в консоли, однако коннект к хабу всё же идёт (через Server Side Events))
            let connection = new HubConnectionBuilder().withUrl('/api/real_time_hub', {
                                                                accessTokenFactory: () => token
                                                        }).build()
            connection.on("UpdateOnSolutionActualResult", async (actualResult) => {
                await dispatch("addSolutionActualResult", actualResult)
            })
            connection.start()
            commit('setHubConnection', connection)
        },
        closeHub({ commit, state, dispatch, getters, rootGetters }) {
            if (!getters.hubConnection) {
                return
            }
            let connection = _.cloneDeep(getters.hubConnection)
            connection.stop()
            commit('setHubConnection', null)
        },
        addInvoke({ commit, state, dispatch, getters, rootGetters }, { method, data }) {
            if (!method || !data) {
                return
            }
            let connection = _.cloneDeep(getters.hubConnection)
            connection.invoke(method, data)
            commit('setHubConnection', connection)
        },
        async addSolutionActualResult({ commit, state, dispatch, getters, rootGetters }, actual_result) {
            if (!actual_result || !rootGetters.currentUser) {
                return
            }
            let solutions = _.cloneDeep(rootGetters.currentContestSolutionsForCurrentUser || [])
            let index = _.findIndex(solutions, (s) => +s.id === +actual_result.solutionId)
            console.log(solutions)
            console.log(index)
            if (+index > -1) {
                solutions[index].actualResult = _.cloneDeep(actual_result)
                commit('setCurrentContestSolutionsForCurrentUser', solutions)
            }
            else {
                solutions = await dispatch('getUserSolutionsInContest', {
                    contest_id: vm.contest_id,
                    user_id: vm.currentUser?.id
                })
                commit('setCurrentContestSolutionsForCurrentUser', solutions)
            }
        },
    }
}