import {createStore} from 'vuex'
import auth from './modules/auth'
import admin from './modules/admin'
import blogs from './modules/blogs'
import contests from './modules/contests'
import checkers from './modules/checkers'
import tasks from './modules/tasks'
import rulesets from './modules/rulesets'
import solutions from './modules/solutions'
import moder_posts from './modules/moder/posts'
import moder_checkers from './modules/moder/checkers'
import moder_contests from './modules/moder/contests'
import moder_courses from './modules/moder/courses'
import moder_problems from './modules/moder/problems'

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
        solutions,
        moder_posts,
        moder_checkers,
        moder_contests,
        moder_courses,
        moder_problems,
    },
    actions: {
        globalAlert({commit, state, dispatch, getters}, {message}) {
            if (message) {
                alert(message)
            }
        },
    }
})