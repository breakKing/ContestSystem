import ManagerPage from "../../views/moder/ManagerPage";
import PostsRoutes from "./manager_routes/posts_routes";
import CheckersRoutes from "./manager_routes/checkers_routes";
import ContestsRoutes from "./manager_routes/contests_routes";
import CoursesRoutes from "./manager_routes/courses_routes";
import ProblemsRoutes from "./manager_routes/problems_routes";
import ManagerStartPage from "../../views/moder/ManagerStartPage";

export default {
    path: '/manager',
    component: ManagerPage,
    meta: {
        authorize: ['moderator'],
        hide_sidebar: false
    },
    children: [
        {
            path: 'home',
            name: 'ManagerHome',
            component: ManagerStartPage,
            meta: {
                authorize: ['moderator'],
                hide_sidebar: false
            },
        },
        ...PostsRoutes,
        ...CheckersRoutes,
        ...ContestsRoutes,
        ...CoursesRoutes,
        ...ProblemsRoutes,
    ]
}