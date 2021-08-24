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
            if (!actual_result || !rootGetters.currentUser) {
                return
            }
            let solutionFromDB;
            // Обновление решений текущего пользователя
            let index = _.findIndex(rootGetters.currentContestSolutionsForCurrentUser, (s) => +s.id === +actual_result.solutionId)
            let props, solution_user_id;
            if (+index > -1) {
                solution_user_id = rootGetters.currentContestSolutionsForCurrentUser[index].participant.id
                props = {actualResult: actual_result}
            } else {
                let solutionFromDB = await dispatch('getSolution', +actual_result.solutionId)
                if (solutionFromDB) {
                    index = rootGetters.currentContestSolutionsForCurrentUser.length;
                    props = solutionFromDB;
                    solution_user_id = solutionFromDB.participant.id
                }
            }
            if (index && solution_user_id && +solution_user_id === +rootGetters.currentUser.id) {
                commit('updateCurrentContestSolutionForCurrentUser', {index, props})
            }

            // обновление решений для организатора
            props = {}
            solution_user_id = null
            index = _.findIndex(rootGetters.currentContestAllSolutions, (s) => +s.id === +actual_result.solutionId)
            if (+index > -1) {
                solution_user_id = rootGetters.currentContestAllSolutions[index].participant.id
                props = {actualResult: actual_result}
            } else {
                if (!solutionFromDB) {
                    solutionFromDB = await dispatch('getSolution', +actual_result.solutionId)
                }
                if (solutionFromDB) {
                    index = rootGetters.currentContestAllSolutions.length;
                    props = solutionFromDB;
                    solution_user_id = solutionFromDB.participant.id
                }
            }
            if (index) {
                commit('updateCurrentContestAllSolutions', {index, props})
            }
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