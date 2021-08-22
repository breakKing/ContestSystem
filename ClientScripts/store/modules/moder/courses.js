import api from "../../../services/api-configurator";
import * as _ from "lodash";

export default {
    namespaced: true,
    state: () => ({
        moderating_course: null,
        courses_to_moderate: [],
        approved_courses: [],
        rejected_courses: [],
    }),
    mutations: {
        setCurrentModeratingCourse(state, val) {
            state.moderating_course = val
        },
        setCoursesToModerate(state, val) {
            state.courses_to_moderate = (val || [])
        },
        setApprovedCourses(state, val) {
            state.approved_courses = (val || [])
        },
        setRejectedCourses(state, val) {
            state.rejected_courses = (val || [])
        },
    },
    getters: {
        currentModeratingCourse(state, getters) {
            return state.moderating_course
        },
        coursesToModerate(state, getters) {
            return state.courses_to_moderate
        },
        approvedCourses(state, getters) {
            return state.approved_courses
        },
        rejectedCourses(state, getters) {
            return state.rejected_courses
        },
        allCourses(state, getters) {
            return _.concat(getters.coursesToModerate, getters.approvedCourses, getters.rejectedCourses)
        }
    },
    actions: {
        async changeCurrentCourse({commit, state, dispatch, getters}, {force, course_id}) {
            await dispatch('fetchCoursesToModerate', force)
            await dispatch('fetchApprovedCourses', force)
            await dispatch('fetchRejectedCourses', force)
            if (!force && +getters.currentModeratingCourse?.id === +course_id) {
                return
            }
            let course = await dispatch('getWorkspaceCourse', course_id, { root: true })
            commit('setCurrentModeratingCourse', course)
        },
        async fetchCoursesToModerate({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.courses_to_moderate && state.courses_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/workspace/courses/requests')
                commit('setCoursesToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedCourses({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.approved_courses && state.approved_courses.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/workspace/courses/accepted')
                commit('setApprovedCourses', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedCourses({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!force && state.rejected_courses && state.rejected_courses.length > 0) {
                return
            }
            try {
                let {data} = await rootGetters.api.get('/workspace/courses/rejected')
                commit('setRejectedCourses', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderateCourse({commit, state, dispatch, getters, rootGetters}, {course_id, request_body}) {
            try {
                let {data} = await rootGetters.api.put(`/workspace/courses/${course_id}/moderate`, request_body)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}