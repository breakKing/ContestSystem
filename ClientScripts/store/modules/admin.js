import axios from 'axios'
import * as _ from 'lodash'

export default {
    state: () => ({
        all_users: [],
    }),
    mutations: {
        setAllUsers(state, val) {
            state.all_users = val || []
        },
    },
    getters: {
        getUserById(state) {
            return (id) => {
                return _.find(state.all_users, (u) => u.id === id)
            }
        }
    },
    actions: {
        async fetchAllUsers({commit, state, dispatch, getters}, force = false) {
            if (!force && state.all_users && state.all_users.length > 0) {
                return
            }
            try {
                let {data} = await axios.post('/api/auth/users/get-all-users', {})
                commit('setAllUsers', data.users)
            } catch (e) {
                console.error(e)
            }
        },
        async updateUser({commit, state, dispatch, getters}, params) {
            try {
                let {data} = await axios.post('/api/auth/users/update-user', params)
                if (data.success) {
                    await dispatch('fetchAllUsers', true)
                } else if (data.errors) {
                    console.error(data.errors)
                }
            } catch (e) {
                console.error(e)
            }
        },
    }
}