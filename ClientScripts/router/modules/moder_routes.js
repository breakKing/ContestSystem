import ManagerPage from "../../views/moder/ManagerPage";
import PostsRoutes from "./manager_routes/posts_routes";

export default {
    path: '/manager',
    name: 'ManagerHome',
    component: ManagerPage,
    meta: {
        authorize: ['manager'],
        hide_sidebar: false
    },
    children: [
        ...PostsRoutes,
    ]
}