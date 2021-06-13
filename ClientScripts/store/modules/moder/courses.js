import axios from 'axios'
import ApproveTypes from "../../../dictionaries/ApproveTypes";
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
        changeCurrentCourse({commit, state, dispatch, getters}, {force, course_id}) {
            if (!force && +getters.currentModeratingCourse?.id === +course_id) {
                return
            }
            commit('setCurrentModeratingCourse', _.find(getters.allCourses, (el) => +el.id === +course_id))
        },
        async fetchCoursesToModerate({commit, state, dispatch, getters}, force = false) {
            if (!force && state.courses_to_moderate && state.courses_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/courses/get-requests')
                commit('setCoursesToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedCourses({commit, state, dispatch, getters}, force = false) {
            if (!force && state.approved_courses && state.approved_courses.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/courses/get-approved')
                commit('setApprovedCourses', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedCourses({commit, state, dispatch, getters}, force = false) {
            if (!force && state.rejected_courses && state.rejected_courses.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/courses/get-rejected')
                commit('setRejectedCourses', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderateCourse({commit, state, dispatch, getters}, {course_id, request_body}) {
            try {
                let {data} = await axios.put(`/api/courses/moderate/${course_id}`, request_body)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}