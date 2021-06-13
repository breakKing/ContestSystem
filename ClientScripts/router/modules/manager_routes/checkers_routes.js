import ModeratorNotModeratedCheckersPage from "../../../views/moder/checkers/ModeratorNotModeratedCheckersPage";
import ModeratorApprovedCheckersPage from "../../../views/moder/checkers/ModeratorApprovedCheckersPage";
import ModeratorRejectedCheckersPage from "../../../views/moder/checkers/ModeratorRejectedCheckersPage";
import ModeratorCheckerModerationPage from "../../../views/moder/checkers/ModeratorCheckerModerationPage";

export default [
    {
        path: 'checkers/not-moderated',
        name: 'ModeratorNotModeratedCheckersPage',
        component: ModeratorNotModeratedCheckersPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'checkers/approved',
        name: 'ModeratorApprovedCheckersPage',
        component: ModeratorApprovedCheckersPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'checkers/rejected',
        name: 'ModeratorRejectedCheckersPage',
        component: ModeratorRejectedCheckersPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'checkers/:checker_id',
        name: 'ModeratorCheckerModerationPage',
        component: ModeratorCheckerModerationPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: true
        },
    },
]
