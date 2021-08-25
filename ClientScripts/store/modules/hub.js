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
                    connection.on("UpdateOnUserStats", async (stats) => {
                        await dispatch("updateUserStats", stats)
                    })
                    connection.on("UpdateOnChatHistory", async (history_entry) => {
                        if (history_entry) {
                            let chat_id = history_entry.chatId
                            commit('updateHistoryEntriesInChat', {chat_id, history_entry})
                        }
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
            if (!actual_result || !rootGetters.currentUser || !rootGetters.currentContest || +rootGetters.currentContest.id !== +actual_result.contestId) {
                return
            }
            let solution_data = await dispatch('updateOrAddSolutionToState', {
                current_solutions_collection: rootGetters.currentContestSolutionsForCurrentUser,
                solution_data: {actualResult: actual_result},
                is_solution_data_full: false,
                update_callback: ({index, props, solution_user_id}) => {
                    if (+index > -1 && solution_user_id && +solution_user_id === +rootGetters.currentUser.id) {
                        commit('updateCurrentContestSolutionForCurrentUser', {index, props})
                    }
                    return props
                }
            })

            // обновление решений для организатора
            if (!solution_data || !_.isEmpty(solution_data)) {
                solution_data = {actualResult: actual_result}
            }
            await dispatch('updateOrAddSolutionToState', {
                current_solutions_collection: rootGetters.currentContestAllSolutions,
                solution_data: solution_data,
                // если есть что-то кроме поля actualResult, то это реальный объект решения
                is_solution_data_full: !!solution_data.id,
                update_callback: ({index, props}) => {
                    if (+index > -1) {
                        commit('updateCurrentContestAllSolutions', {index, props})
                    }
                    return props
                }
            })
        },
        async updateUserStats({commit, state, dispatch, getters, rootGetters}, stats) {
            if (!stats || !rootGetters.currentUser || !rootGetters.currentContest) {
                return
            }

            if (+rootGetters.currentContest.id === +stats.contestId) {
                commit('setCurrentContestUserStats', stats)
            }
        }
    }
}