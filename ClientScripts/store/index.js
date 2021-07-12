import {createStore} from 'vuex'
import auth from './modules/auth'
import admin from './modules/admin'
import blogs from './modules/blogs'
import contests from './modules/contests'
import checkers from './modules/checkers'
import tasks from './modules/tasks'
import rulesets from './modules/rulesets'
import solutions from './modules/solutions'
import hub from "./modules/hub"
import moder_posts from './modules/moder/posts'
import moder_checkers from './modules/moder/checkers'
import moder_contests from './modules/moder/contests'
import moder_courses from './modules/moder/courses'
import moder_problems from './modules/moder/problems'
import ApproveTypes from "../dictionaries/ApproveTypes";
import moment from "moment";

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
        hub,
        moder_posts,
        moder_checkers,
        moder_contests,
        moder_courses,
        moder_problems,
    },
    getters: {
        approvalStatuses(state, getters) {
            return ApproveTypes
        },
        getFormattedFullDateTime: (state, getters) => (date) => {
            moment.locale('ru');
            return moment.utc(date).local().format('LLLL');
        },
        getFormattedMemory: (state, getters) => (bytes) => {
            let result = '0 Б'
            if (bytes) {
                if (+bytes < 1024) {
                    result = bytes + ' Б'
                }
                else if (+bytes < 1048576) {
                    result = (+bytes / 1024).toFixed(1) + ' КБ'
                }
                else {
                    result = (+bytes / 1048576).toFixed(1) + ' МБ'
                }
                result = result.replace('.0', '')
            }
            return result
        },
        getFormattedTime: (state, getters) => (millis) => {
            let result = '0 мс'
            if (millis) {
                if (+millis < 1000) {
                    result = millis + ' мс'
                }
                else {
                    result = (+millis / 1000).toFixed(1) + ' с'
                }
                result = result.replace('.0', '')
            }
            return result
        }
    },
    actions: {
        globalAlert({commit, state, dispatch, getters}, {message}) {
            if (message) {
                alert(message)
            }
        },
    }
})