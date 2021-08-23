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
import chats from "./modules/chats";

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
        chats,
        moder_posts,
        moder_checkers,
        moder_contests,
        moder_courses,
        moder_problems,
    },
    state: {
        api: null
    },
    mutations: {
        setApi(state, val) {
            state.api = val
        }
    },
    getters: {
        api(state, getters) {
            return state.api
        },
        approvalStatuses(state, getters) {
            return ApproveTypes
        },
        getFormattedFullDateTime: (state, getters) => (date) => {
            moment.locale('ru');
            return moment.utc(date).local().format('LLLL');
        },
        getFormattedMemory: (state, getters) => (bytes) => {
            let result = ''
            if (bytes) {
                if (+bytes < 1024) {
                    result = bytes + ' Б'
                } else if (+bytes < 1048576) {
                    result = (+bytes / 1024).toFixed(1) + ' КБ'
                } else {
                    result = (+bytes / 1048576).toFixed(1) + ' МБ'
                }
                result = result.replace('.0', '')
            }
            return result
        },
        getFormattedTime: (state, getters) => (millis) => {
            let result = ''
            if (millis) {
                if (+millis < 1000) {
                    result = millis + ' мс'
                } else if (+millis < 60000) {
                    result = (+millis / 1000).toFixed(1) + ' с'
                } else {
                    let mls = +millis % 1000
                    let seconds = ((+millis - mls) % 60000) / 1000
                    let minutes = ((+millis - mls - seconds * 1000) / 60000) % 60
                    let hours = (+millis - mls - seconds * 1000 - minutes * 60000) / 3600000
                    result = (+hours > 9 ? hours.toFixed(0) : '0' + hours.toFixed(0)) + ':' + (+minutes > 9 ? minutes.toFixed(0) : '0' + minutes.toFixed(0))
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