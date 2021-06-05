import {createStore} from 'vuex'
import auth from './modules/auth'
import admin from './modules/admin'
import blogs from './modules/blogs'
import contests from './modules/contests'
import checkers from './modules/checkers'
import tasks from './modules/tasks'
import rulesets from './modules/rulesets'

export default createStore({
    strict: process.env.NODE_ENV !== 'production',
    modules: {
        auth,
        admin,
        blogs,
        contests,
        checkers,
        tasks,
        rulesets,
    },
    actions: {
        globalAlert({commit, state, dispatch, getters}, {message}) {
            if (message) {
                alert(message)
            }
        },
    }
})