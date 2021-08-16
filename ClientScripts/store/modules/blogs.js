import axios from 'axios'
import * as _ from 'lodash'
import $ from "jquery";
import ApproveTypes from "../../dictionaries/ApproveTypes";
import moment from "moment";

export default {
    state: () => ({
        posts_list: [],
        current_user_posts_list: [],
    }),
    mutations: {
        setPostsList(state, val) {
            state.posts_list = (val || [])
        },
        setCurrentUserPosts(state, val) {
            state.current_user_posts_list = (val || [])
        },
    },
    getters: {
        postsList(state, getters) {
            return state.posts_list
        },
        currentUserPostsList(state, getters) {
            return state.current_user_posts_list
        },
        currentUserPendingPostsList(state, getters) {
            return _.filter(getters.currentUserPostsList, (el) => +el.approvalStatus === ApproveTypes.NotModeratedYet)
        },
        currentUserRejectedPostsList(state, getters) {
            return _.filter(getters.currentUserPostsList, (el) => +el.approvalStatus === ApproveTypes.Rejected)
        },
        currentUserApprovedPostsList(state, getters) {
            return _.filter(getters.currentUserPostsList, (el) => +el.approvalStatus === ApproveTypes.Accepted)
        },
        latestPosts(state, getters) {
            return _.orderBy((getters.postsList || []), [(p)=> +moment(p.publicationDateTimeUTC).format('x')],['desc'] )
        },
    },
    actions: {
        async fetchPostsList({commit, state, dispatch, getters}, force = false) {
            if (!force && state.posts_list && state.posts_list.length > 0) {
                return
            }
            try {
                let {data} = await axios.get('/api/blog/posts/ru')
                commit('setPostsList', data)
            } catch (e) {
                console.error(e)
            }
        },
        async getWorkspacePost({ commit, state, dispatch, getters }, post_id) {
            if (!post_id) {
                return null
            }
            try {
                let { data } = await axios.get(`/api/workspace/posts/${post_id}`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async getPostInfo({commit, state, dispatch, getters}, blog_id) {
            if (!blog_id) {
                return null
            }
            try {
                let {data} = await axios.get(`/api/blog/posts/${blog_id}/ru`)
                return data
            } catch (e) {
                console.error(e)
                return null
            }
        },
        async savePostInfo({commit, state, dispatch, getters}, {request_data, post_id}) {
            try {
                if (post_id) {
                    return await $.ajax({
                        url: `/api/workspace/posts/${post_id}`,
                        data: request_data,
                        processData: false,
                        contentType: false,
                        type: 'PUT'
                    })
                } else {
                    return await $.ajax({
                        url: `/api/workspace/posts`,
                        data: request_data,
                        processData: false,
                        contentType: false,
                        type: 'POST',
                    })
                }
            } catch (e) {
                console.error(e)
                return {}
            }
        },
        async fetchUserPostsList({commit, state, dispatch, getters, rootGetters}, force = false) {
            if (!rootGetters.currentUser) {
                return
            }
            if (!force && state.current_user_posts_list && state.current_user_posts_list.length > 0) {
                return
            }
            try {
                let {data} = await axios.get(`/api/workspace/posts/user/${rootGetters.currentUser.id}/ru`)
                commit('setCurrentUserPosts', data)
            } catch (e) {
                console.error(e)
            }
        },
        async deletePost({commit, state, dispatch, getters}, post_id) {
            try {
                let {data} = await axios.delete(`/api/workspace/posts/${post_id}`)
                return data
            } catch (e) {
                console.error(e)
            }
            return {}
        },
    }
}