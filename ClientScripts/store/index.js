import {createStore} from 'vuex'
import auth from './modules/auth'
import admin from './modules/admin'

export default createStore({
    strict: process.env.NODE_ENV !== 'production',
    modules: {
        auth,
        admin,
    },
    state: () => ({
    }),
    mutations: {},
    getters: {},
    actions: {
        globalAlert({commit, state, dispatch, getters}, {message}) {
            if (message) {
                alert(message)
            }
        },
    }
})