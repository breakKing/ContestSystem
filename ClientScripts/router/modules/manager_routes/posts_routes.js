import ModeratorNotModeratedPostsPage from "../../../views/moder/posts/ModeratorNotModeratedPostsPage";
import ModeratorApprovedPostsPage from "../../../views/moder/posts/ModeratorApprovedPostsPage";
import ModeratorRejectedPostsPage from "../../../views/moder/posts/ModeratorRejectedPostsPage";
import ModeratorPostModerationPage from "../../../views/moder/posts/ModeratorPostModerationPage";

export default [
    {
        path: 'posts/not-moderated',
        name: 'ModeratorNotModeratedPostsPage',
        component: ModeratorNotModeratedPostsPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'posts/approved',
        name: 'ModeratorApprovedPostsPage',
        component: ModeratorApprovedPostsPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'posts/rejected',
        name: 'ModeratorRejectedPostsPage',
        component: ModeratorRejectedPostsPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'posts/:post_id',
        name: 'ModeratorPostModerationPage',
        component: ModeratorPostModerationPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: true
        },
    },
]
