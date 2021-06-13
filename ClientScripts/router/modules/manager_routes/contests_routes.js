import ModeratorNotModeratedContestsPage from "../../../views/moder/contests/ModeratorNotModeratedContestsPage";
import ModeratorApprovedContestsPage from "../../../views/moder/contests/ModeratorApprovedContestsPage";
import ModeratorRejectedContestsPage from "../../../views/moder/contests/ModeratorRejectedContestsPage";
import ModeratorContestModerationPage from "../../../views/moder/contests/ModeratorContestModerationPage";

export default [
    {
        path: 'contests/not-moderated',
        name: 'ModeratorNotModeratedContestsPage',
        component: ModeratorNotModeratedContestsPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'contests/approved',
        name: 'ModeratorApprovedContestsPage',
        component: ModeratorApprovedContestsPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'contests/rejected',
        name: 'ModeratorRejectedContestsPage',
        component: ModeratorRejectedContestsPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'contests/:contest_id',
        name: 'ModeratorContestModerationPage',
        component: ModeratorContestModerationPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: true
        },
    },
]
