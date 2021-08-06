import {HubConnectionBuilder} from "@microsoft/signalr"
import * as _ from "lodash"

export default {
    state: () => ({
        hub_connection: null,
    }),
    mutations: {
        setHubConnection(state, val) {
            if (state.hub_connection && state.hub_connection !== val) {
                state.hub_connection.stop()
            }
            state.hub_connection = val
        },
        invokeInHubConnection(state, {method, data}) {
            state.hub_connection.invoke(method, data)
        }
    },
    getters: {
        hubConnection(state, getters) {
            return state.hub_connection
        },
    },
    actions: {
        async initHub({commit, state, dispatch, getters, rootGetters}, {token, recreate}) {
            if (recreate || !getters.hubConnection) {
                if (recreate) {
                    await dispatch('closeHub')
                }
                if (token) {
                    let connection = new HubConnectionBuilder().withUrl('/api/real_time_hub', {
                        accessTokenFactory: () => token
                    }).withAutomaticReconnect()
                      .build()
                    connection.on("UpdateOnSolutionActualResult", async (actualResult) => {
                        await dispatch("addSolutionActualResult", actualResult)
                    })
                    await connection.start()
                    commit('setHubConnection', connection)
                }
            }
        },
        closeHub({commit, state, dispatch, getters, rootGetters}) {
            if (!getters.hubConnection) {
                return
            }
            commit('setHubConnection', null)
        },
        addInvoke({commit, state, dispatch, getters, rootGetters}, {method, data}) {
            if (!method || !data) {
                return
            }
            commit('invokeInHubConnection', {method, data})
        },
        async addSolutionActualResult({commit, state, dispatch, getters, rootGetters}, actual_result) {
            if (!actual_result || !rootGetters.currentUser) {
                return
            }
            let solutions = _.cloneDeep(rootGetters.currentContestSolutionsForCurrentUser || [])
            let index = _.findIndex(solutions, (s) => +s.id === +actual_result.solutionId)
            if (+index > -1) {
                solutions[index].actualResult = actual_result
            } else {
                solutions = await dispatch('getUserSolutionsInContest', {
                    contest_id: rootGetters.currentContest?.id,
                    user_id: rootGetters.currentUser?.id
                })
            }
            commit('setCurrentContestSolutionsForCurrentUser', solutions)
        },
    }
}