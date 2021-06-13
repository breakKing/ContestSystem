import ModeratorNotModeratedCoursesPage from "../../../views/moder/courses/ModeratorNotModeratedCoursesPage";
import ModeratorApprovedCoursesPage from "../../../views/moder/courses/ModeratorApprovedCoursesPage";
import ModeratorRejectedCoursesPage from "../../../views/moder/courses/ModeratorRejectedCoursesPage";
import ModeratorCourseModerationPage from "../../../views/moder/courses/ModeratorCourseModerationPage";

export default [
    {
        path: 'courses/not-moderated',
        name: 'ModeratorNotModeratedCoursesPage',
        component: ModeratorNotModeratedCoursesPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'courses/approved',
        name: 'ModeratorApprovedCoursesPage',
        component: ModeratorApprovedCoursesPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'courses/rejected',
        name: 'ModeratorRejectedCoursesPage',
        component: ModeratorRejectedCoursesPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: false
        },
    },
    {
        path: 'courses/:course_id',
        name: 'ModeratorCourseModerationPage',
        component: ModeratorCourseModerationPage,
        props: true,
        meta: {
            authorize: ['manager'],
            hide_sidebar: true
        },
    },
]
