import * as _ from 'lodash'
import $ from "jquery";
import generateFingerprint from '../../services/fingerprint-loader';

export default {
    state: () => ({
        token: null,
        fingerprint: null,
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
        setToken(state, val) {
            state.token = val
        },
        setFingerprint(state, val) {
            state.fingerprint = val
        },
        setUserRoles(state, val) {
            state.user_roles = val
        },
        setCurrentUser(state, val) {
            state.current_user = val
        }
    },
    getters: {
        accessToken(state, getters) {
            return state.token
        },
        fingerprint(state, getters) {
            return state.fingerprint
        },
        userName(state, getters) {
            return getters.currentUser?.userName
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
        async getFingerprint({commit}) {
            let print = String(await generateFingerprint())
            commit('setFingerprint', print)
        },
        async updateTokenAndHeaders({commit, state, dispatch, getters}, token) {
            commit('setToken', token)
            await dispatch('initHub', {token, recreate: true})

            $.ajaxSetup({
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Authorization', getters.getHeaders['Authorization']);
                }
            });
        },
        async initAuth({commit, state, dispatch, getters}) {
            if (!state.auth_initialized) {
                // возможно авторизован
                await dispatch('refreshToken')

                commit('setAuthInitializationStatus', true)
            }
        },
        async refreshToken({commit, state, dispatch, getters, rootGetters}) {
            try {
                if (!getters.fingerprint) {
                    await dispatch('getFingerprint')
                }
                let {data} = await rootGetters.api.post('/auth/session/verify-token', {fingerprint: getters.fingerprint})
                if (data.token) {
                    await dispatch('updateTokenAndHeaders', data.token)
                    commit('setCurrentUser', data.user)
                    commit('setUserRoles', data.roles)
                }
            }
            catch (e) {
                console.error(e)
                await dispatch('logout')
            }
        },
        async sendLoginRequest({commit, state, dispatch, getters, rootGetters}, {username, password}) {
            try {
                if (!getters.fingerprint) {
                    await dispatch('getFingerprint')
                }
                // TODO реализовать кнопку "запомнить меня"
                let {data} = await rootGetters.api.post('/auth/session/login', {
                    username,
                    password,
                    remember: false,
                    fingerprint: getters.fingerprint
                })
                if (data.status) {
                    await dispatch('updateTokenAndHeaders', data.token)
                    commit('setCurrentUser', data.user)
                    commit('setUserRoles', data.roles)
                    commit('setAuthError', null)

                    return true
                } else {
                    commit('setAuthError', data.errors[0])
                    await dispatch('logout')
                    return false
                }
            } catch (e) {
                console.error(e)
                await dispatch('logout')

                return false
            }
        },
        async sendRegistrationRequest({commit, state, dispatch, getters, rootGetters}, request_data) {
            try {
                let {data} = await rootGetters.api.post('/auth/session/register', request_data)
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
        async logout({ commit, state, dispatch, getters, rootGetters }) {
            if (!getters.currentUser) {
                return;
            }
            await rootGetters.api.post('/auth/session/logout', { fingerprint: getters.fingerprint })
            await dispatch('updateTokenAndHeaders', null)

            commit('setCurrentUser', null)
            commit('setUserRoles', null)

            commit('setRunningContests', [], {root: true})
            commit('setAvailableContests', [], {root: true})
            commit('setParticipatingContests', [], {root: true})
            commit('setAvailableCheckers', [], {root: true})

            commit('setCurrentUserCheckers', [], {root: true})
            commit('setCurrentUserPosts', [], {root: true})
            commit('setCurrentUserContests', [], {root: true})
            commit('setCurrentUserRuleSets', [], {root: true})
            commit('setCurrentUserTasks', [], {root: true})
            
            commit('setCurrentUserChats', [], {root: true})
            
            await dispatch('closeHub', null, {root: true})
            await dispatch('changeCurrentContest', {force: true, contest_id: null}, {root: true})
        },
        async fetchAllRoles({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.all_roles && state.all_roles.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.post('/auth/session/get-all-roles', {})
                commit('setAllRolesArray', data.roles)
            } catch (e) {
                console.error(e)
            }
        }
    }
}