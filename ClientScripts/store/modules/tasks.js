import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        current_user_tasks: [],
        available_for_select_tasks: [],
    }),
    mutations: {
        setCurrentUserTasks(state, val) {
            state.current_user_tasks = (val || [])
        },
        setAvailableTasks(state, val) {
            state.available_for_select_tasks = (val || [])
        },
    },
    getters: {
        availableTasks(state, getters) {
            return state.available_for_select_tasks
        },
        currentUserTasks(state, getters) {
            return state.current_user_tasks
        },
        currentUserPendingTasks(state, getters) {
            return _.filter(getters.currentUserTasks, (el) => Number(el.approvalStatus) === 0)
        },
        currentUserRejectedTasks(state, getters) {
            return _.filter(getters.currentUserTasks, (el) => Number(el.approvalStatus) === 1)
        },
        currentUserApprovedTasks(state, getters) {
            return _.filter(getters.currentUserTasks, (el) => Number(el.approvalStatus) === 2)
        },
    },
    actions: {
        async fetchCurrentUserTasks({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.current_user_tasks && state.current_user_tasks.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/problems/get-user-problems/${rootGetters.currentUser.id}/ru`)
                commit('setCurrentUserTasks', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchAvailableTasks({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.available_for_select_tasks && state.available_for_select_tasks.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/problems/get-available-problems/${rootGetters.currentUser.id}/ru`)
                commit('setAvailableTasks', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getTask({commit, state, dispatch, getters, rootGetters}, task_id) {
            if (!task_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/problems/constructed/${task_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return null
        },
        async deleteTask({commit, state, dispatch, getters, rootGetters}, task_id) {
            try {
                let {data} = await axios.get(`/api/problems/delete-problem/${task_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}