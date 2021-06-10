import UserPage from "../../views/user/UserPage";
import UserStarterPage from "../../views/user/UserStarterPage";
import PostsPage from "../../views/user/PostsPage";
import CoursePage from "../../views/user/CoursePage";
import PostViewComponent from "../../components/user/blogs/PostViewComponent";
import ContestsPage from "../../views/user/contests/ContestsPage";
import AvailableContestsComponent from "../../components/user/contests/AvailableContestsComponent";
import ParticipatingContestsComponent from "../../components/user/contests/ParticipatingContestsComponent";
import CurrentlyRunningContestsComponent from "../../components/user/contests/CurrentlyRunningContestsComponent";
import WorkSpaceRoutes from './user_routes/workspace_routes';

export default {
    path: '/user',
    name: 'UserHome',
    component: UserPage,
    children: [
        {
            path: '',
            name: 'UserStarterPage',
            component: UserStarterPage,
        },
        {
            path: 'posts',
            name: 'PostsPage',
            component: PostsPage,
        },
        {
            path: 'courses',
            name: 'CoursePage',
            component: CoursePage,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: 'post/:post_id',
            name: 'ViewPost',
            component: PostViewComponent,
            props: true,
        },
        {
            path: 'contests',
            name: 'ContestsPage',
            component: ContestsPage,
            meta: {
                authorize: ['user']
            },
            children: [
                {
                    path: 'available-to-participate',
                    name: 'AvailableContestsPage',
                    component: AvailableContestsComponent,
                    meta: {
                        authorize: ['user']
                    },
                },
                {
                    path: 'participating',
                    name: 'ParticipatingContestsPage',
                    component: ParticipatingContestsComponent,
                    meta: {
                        authorize: ['user']
                    },
                },
                {
                    path: 'running-now',
                    name: 'CurrentlyRunningContestsComponentPage',
                    component: CurrentlyRunningContestsComponent,
                    meta: {
                        authorize: ['user']
                    },
                },
            ]
        },
        WorkSpaceRoutes,
    ],
}