import ModeratorNotModeratedProblemsPage from "../../../views/moder/problems/ModeratorNotModeratedProblemsPage";
import ModeratorApprovedProblemsPage from "../../../views/moder/problems/ModeratorApprovedProblemsPage";
import ModeratorRejectedProblemsPage from "../../../views/moder/problems/ModeratorRejectedProblemsPage";
import ModeratorProblemModerationPage from "../../../views/moder/problems/ModeratorProblemModerationPage";

export default [
    {
        path: 'problems/not-moderated',
        name: 'ModeratorNotModeratedProblemsPage',
        component: ModeratorNotModeratedProblemsPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: false
        },
    },
    {
        path: 'problems/approved',
        name: 'ModeratorApprovedProblemsPage',
        component: ModeratorApprovedProblemsPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: false
        },
    },
    {
        path: 'problems/rejected',
        name: 'ModeratorRejectedProblemsPage',
        component: ModeratorRejectedProblemsPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: false
        },
    },
    {
        path: 'problems/:problem_id',
        name: 'ModeratorProblemModerationPage',
        component: ModeratorProblemModerationPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: true
        },
    },
]
