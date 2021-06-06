import axios from 'axios'
import * as _ from 'lodash'
import $ from "jquery";

export default {
    state: () => ({
        token: localStorage.getItem('auth-token') || null,
        user_roles: [],
        all_roles: [],
        current_user: null,
        selected_role: null,
        auth_initialized: false,
        auth_error: null,
        potential_headers: ['Authorization', 'Content-Type']
    }),
    mutations: {
        setAllRolesArray(state, val) {
            state.all_roles = (val || [])
        },
        setCurrentRole(state, val) {
            state.selected_role = val
        },
        setAuthInitializationStatus(state, val) {
            state.auth_initialized = val
        },
        setAuthError(state, val) {
            state.auth_error = val
        },
        setToken(state, newValue) {
            if (!newValue) {
                localStorage.removeItem('auth-token')
            } else {
                localStorage.setItem('auth-token', newValue)
            }
            state.token = newValue
        },
        setUserRoles(state, val) {
            state.user_roles = val
        },
        setCurrentUser(state, val) {
            state.current_user = val
        }
    },
    getters: {
        userName(state, getters) {
            return getters.currentUser?.fullName
        },
        currentUser(state, getters) {
            if (!getters.isAuthenticated) {
                return null
            }
            return state.current_user
        },
        currentRole(state, getters) {
            if (!getters.isAuthenticated) {
                return null
            }
            if (!state.selected_role) {
                let currentUserRoles = getters.currentUserRoles
                if (currentUserRoles && currentUserRoles.length === 1) {
                    return currentUserRoles[0]
                }
            }
            return state.selected_role
        },
        mappedRoles(state) {
            return _.keyBy(state.all_roles, 'name')
        },
        getHeaders(state) {
            const authHeader = state.token ? {'Authorization': 'Bearer ' + state.token} : {}
            return {
                ...authHeader,
                'Content-Type': 'application/json'
            }
        },
        isAuthenticated(state) {
            return state.auth_initialized && state.token
        },
        currentUserRoles(state) {
            if (state.auth_initialized && state.user_roles) {
                return state.user_roles
            }
            return []
        },
    },
    actions: {
        updateTokenAndHeaders({commit, state, dispatch, getters}, token) {
            commit('setToken', token)
            axios.defaults.headers.common = getters.getHeaders

            $.ajaxSetup({
                beforeSend: function(xhr) {
                    xhr.setRequestHeader('Authorization', getters.getHeaders['Authorization']);
                }
            });
        },
        async initAuth({commit, state, dispatch, getters}) {
            if (!state.auth_initialized) {
                if (state.token) {
                    // возможно авторизован
                    axios.defaults.headers.common = getters.getHeaders
                    try {
                        let {data} = await axios.post('/api/session/verify-token', {})
                        if (data.token) {
                            await dispatch('updateTokenAndHeaders', data.token)
                            commit('setCurrentUser', data.user)
                            commit('setUserRoles', data.roles)
                        }
                    } catch (e) {
                        console.error(e)
                        await dispatch('logout')
                    }
                }
                commit('setAuthInitializationStatus', true)
            }
        },
        async sendLoginRequest({commit, state, dispatch, getters}, {username, password}) {
            try {
                let {data} = await axios.post('/api/session/login', {
                    username,
                    password
                })
                if (data.status) {
                    await dispatch('updateTokenAndHeaders', data.token)
                    commit('setCurrentUser', data.user)
                    commit('setUserRoles', data.roles)
                    commit('setAuthError', null)

                    return true
                } else {
                    commit('setAuthError', data.message)
                    await dispatch('logout')
                    return false
                }
            } catch (e) {
                console.error(e)
                await dispatch('logout')

                return false
            }
        },
        async sendRegistrationRequest({commit, state, dispatch, getters}, request_data) {
            try {
                let {data} = await axios.post('/api/session/register', request_data)
                if (data.status) {
                    await dispatch('updateTokenAndHeaders', data.token)
                    commit('setCurrentUser', data.user)
                    commit('setUserRoles', data.roles)
                    commit('setAuthError', null)

                    return true
                } else {
                    commit('setAuthError', data.message)

                    return false
                }
            } catch (e) {
                console.error(e)
                await dispatch('logout')

                return false
            }
        },
        async logout({commit, state, dispatch, getters,}) {
            await dispatch('updateTokenAndHeaders', null)
            commit('setCurrentUser', null)
            commit('setUserRoles', null)

            commit('setCurrentUserPosts', [], {root: true})
            commit('setRunningContests', [], {root: true})
            commit('setAvailableContests', [], {root: true})
            commit('setParticipatingContests', [], {root: true})
            commit('setCurrentUserCheckers', [], {root: true})
            commit('setAvailableCheckers', [], {root: true})
        },
        async fetchAllRoles({commit, state, dispatch, getters}, force = false) {
            if (!force && state.all_roles && state.all_roles.length > 0) {
                return
            }
            try {
                let {data} = await axios.post('/api/session/get-all-roles', {})
                commit('setAllRolesArray', data.roles)
            } catch (e) {
                console.error(e)
            }
        }
    }
}