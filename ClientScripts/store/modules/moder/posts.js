import axios from 'axios'
import * as _ from 'lodash'
import ApproveTypes from "../../../dictionaries/ApproveTypes";

export default {
    namespaced: true,
    state: () => ({
        moderating_post: null,
        posts_to_moderate: [],
        approved_posts: [],
        rejected_posts: [],
    }),
    mutations: {
        setCurrentModeratingPost(state, val) {
            state.moderating_post = val
        },
        setPostsToModerate(state, val) {
            state.posts_to_moderate = (val || [])
        },
        setApprovedPosts(state, val) {
            state.approved_posts = (val || [])
        },
        setRejectedPosts(state, val) {
            state.rejected_posts = (val || [])
        },
    },
    getters: {
        currentModeratingPost(state, getters) {
            return state.moderating_post
        },
        currentModeratingPostLocalizer(state, getters) {
            return _.find((getters.currentModeratingPost?.localizers || []), (l) => l.culture === 'ru')
        },
        currentModeratingPostApprovalStatusName(state, getters) {
            if (!getters.currentModeratingPost) {
                return ''
            }
            switch (+getters.currentModeratingPost.approvalStatus) {
                case ApproveTypes.NotModeratedYet:
                    return 'Не проверено'
                case ApproveTypes.Accepted:
                    return 'Подтверждено'
                case ApproveTypes.Rejected:
                    return 'Отклонено'
            }
        },
        postsToModerate(state, getters) {
            return state.posts_to_moderate
        },
        approvedPosts(state, getters) {
            return state.approved_posts
        },
        rejectedPosts(state, getters) {
            return state.rejected_posts
        },
        allPosts(state, getters) {
            return _.concat(getters.postsToModerate, getters.approvedPosts, getters.rejectedPosts)
        }
    },
    actions: {
        async changeCurrentPost({commit, state, dispatch, getters}, {force, post_id}) {
            await dispatch('fetchPostsToModerate', force)
            await dispatch('fetchApprovedPosts', force)
            await dispatch('fetchRejectedPosts', force)
            if (!force && +getters.currentModeratingPost?.id === +post_id) {
                return
            }
            commit('setCurrentModeratingPost', _.find(getters.allPosts, (el) => +el.id === +post_id))
        },
        async fetchPostsToModerate({commit, state, dispatch, getters}, force = false) {
            if (!force && state.posts_to_moderate && state.posts_to_moderate.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/workspace/posts/requests')
                commit('setPostsToModerate', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchApprovedPosts({commit, state, dispatch, getters}, force = false) {
            if (!force && state.approved_posts && state.approved_posts.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/workspace/posts/accepted')
                commit('setApprovedPosts', data)
            } catch (e) {
                console.error(e)
            }
        },
        async fetchRejectedPosts({commit, state, dispatch, getters}, force = false) {
            if (!force && state.rejected_posts && state.rejected_posts.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/workspace/posts/rejected')
                commit('setRejectedPosts', data)
            } catch (e) {
                console.error(e)
            }
        },
        async moderatePost({commit, state, dispatch, getters}, {post_id, request_body}) {
            try {
                let {data} = await axios.put(`/api/workspace/posts/${post_id}/moderate`, request_body)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}