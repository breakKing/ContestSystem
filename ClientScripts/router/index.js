import {createRouter, createWebHistory} from 'vue-router'
import store from '../store/index'
import LoginPage from '../views/LoginPage'
import RegisterPage from '../views/RegisterPage'
import RolesPage from '../views/RolesPage'
import AdminPage from '../views/admin/AdminPage'
import ManagerPage from '../views/moder/ManagerPage'
import UserStarterPage from '../views/user/UserStarterPage'
import UserPage from '../views/user/UserPage'
import PostsPage from '../views/user/PostsPage'
import CoursePage from '../views/user/CoursePage'
import ContestsPage from '../views/user/contests/ContestsPage'
import UsersListComponent from "../components/admin/UsersListComponent";
import AvailablePostsComponent from "../components/user/contests/AvailablePostsComponent";
import ParticipatingContestsComponent from "../components/user/contests/ParticipatingContestsComponent";
import CurrentlyRunningContestsComponent from "../components/user/contests/CurrentlyRunningContestsComponent";
import PostViewComponent from "../components/user/blogs/PostViewComponent";
import WorkSpacePage from "../views/user/workspace/WorkSpacePage";
import WorkSpaceWelcomeComponent from "../components/user/workspace/WorkSpaceWelcomeComponent";
import AboutComponent from "../components/user/workspace/AboutComponent";
import MyPostsComponent from "../components/user/workspace/posts/MyPostsComponent";
import PostsMainComponent from "../components/user/workspace/posts/PostsMainComponent";
import MyPendingPostsComponent from "../components/user/workspace/posts/MyPendingPostsComponent";
import MyRejectedPostsComponent from "../components/user/workspace/posts/MyRejectedPostsComponent";
import MyApprovedPostsComponent from "../components/user/workspace/posts/MyApprovedPostsComponent";
import ContestsMainComponent from "../components/user/workspace/contests/ContestsMainComponent";
import MyPendingContestsComponent from "../components/user/workspace/contests/MyPendingContestsComponent";
import MyRejectedContestsComponent from "../components/user/workspace/contests/MyRejectedContestsComponent";
import MyApprovedContestsComponent from "../components/user/workspace/contests/MyApprovedContestsComponent";
import MyContestsComponent from "../components/user/workspace/contests/MyContestsComponent";
import TasksMainComponent from "../components/user/workspace/tasks/TasksMainComponent";
import MyPendingTasksComponent from "../components/user/workspace/tasks/MyPendingTasksComponent";
import MyRejectedTasksComponent from "../components/user/workspace/tasks/MyRejectedTasksComponent";
import MyApprovedTasksComponent from "../components/user/workspace/tasks/MyApprovedTasksComponent";
import MyTasksComponent from "../components/user/workspace/tasks/MyTasksComponent";
import CheckersMainComponent from "../components/user/workspace/checkers/CheckersMainComponent";
import MyRejectedCheckersComponent from "../components/user/workspace/checkers/MyRejectedCheckersComponent";
import MyPendingCheckersComponent from "../components/user/workspace/checkers/MyPendingCheckersComponent";
import MyApprovedCheckersComponent from "../components/user/workspace/checkers/MyApprovedCheckersComponent";
import MyCheckersComponent from "../components/user/workspace/checkers/MyCheckersComponent";
import MainRuleSetsComponent from "../components/user/workspace/rule_sets/MainRuleSetsComponent";
import RuleSetsListComponent from "../components/user/workspace/rule_sets/RuleSetsListComponent";
import CheckerEditComponent from "../components/user/workspace/checkers/CheckerEditComponent";
import TaskEditComponent from "../components/user/workspace/tasks/TaskEditComponent";
import RuleSetEditComponent from "../components/user/workspace/rule_sets/RuleSetEditComponent";
import AllRuleSetsListComponent from "../components/user/workspace/rule_sets/AllRuleSetsListComponent";
import ContestEditComponent from "../components/user/workspace/contests/ContestEditComponent";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'Home',
            redirect: (to) => {
                // redirect синхронный
                (async () => {
                    await store.dispatch('initAuth') // ensure auth synced with server
                })()
                if (!store.getters.isAuthenticated) {
                    return {name: 'UserStarterPage'}
                }
                let current_role = store.getters.currentRole
                if (!current_role) {
                    return {name: 'RoleSelector'}
                }
                let route_name;
                // разбираем на страницы по ролям
                switch (current_role) {
                    case 'admin':
                        route_name = 'AdminHome';
                        break
                    case 'manager':
                        route_name = 'ManagerHome';
                        break
                    default: // user
                        route_name = 'UserStarterPage';
                        break
                }
                return {name: route_name}
            },
            meta: {
                authorize: true
            }
        },
        {
            path: '/admin',
            name: 'AdminHome',
            component: AdminPage,
            meta: {
                authorize: ['admin']
            },
            children: [
                {
                    path: 'users',
                    name: 'AdminUsersList',
                    component: UsersListComponent,
                    meta: {
                        authorize: ['admin']
                    },
                }
            ],
        },
        {
            path: '/manager',
            name: 'ManagerHome',
            component: ManagerPage,
            meta: {
                authorize: ['manager']
            }
        },
        {
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
                            component: AvailablePostsComponent,
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
                {
                    path: 'workspace',
                    name: 'WorkSpacePage',
                    component: WorkSpacePage,
                    meta: {
                        authorize: ['user']
                    },
                    children: [
                        {
                            path: 'welcome',
                            name: 'WorkSpaceWelcomeComponent',
                            component: WorkSpaceWelcomeComponent,
                            meta: {
                                authorize: ['user']
                            },
                        },
                        {
                            path: 'about',
                            name: 'WorkSpaceAboutPage',
                            component: AboutComponent,
                            meta: {
                                authorize: ['user']
                            },
                        },
                        {
                            path: 'posts',
                            name: 'WorkSpacePostsPage',
                            component: PostsMainComponent,
                            meta: {
                                authorize: ['user']
                            },
                            children: [
                                {
                                    path: 'my-pending-posts',
                                    name: 'WorkSpaceMyPendingPostsPage',
                                    component: MyPendingPostsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-rejected-posts',
                                    name: 'WorkSpaceMyRejectedPostsPage',
                                    component: MyRejectedPostsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-approved-posts',
                                    name: 'WorkSpaceMyApprovedPostsPage',
                                    component: MyApprovedPostsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-posts',
                                    name: 'WorkSpaceMyPostsPage',
                                    component: MyPostsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                            ]
                        },
                        {
                            path: 'contests',
                            name: 'WorkSpaceContestsPage',
                            component: ContestsMainComponent,
                            meta: {
                                authorize: ['user']
                            },
                            children: [
                                {
                                    path: 'my-pending-contests',
                                    name: 'WorkSpaceMyPendingContestsPage',
                                    component: MyPendingContestsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-rejected-contests',
                                    name: 'WorkSpaceMyRejectedContestsPage',
                                    component: MyRejectedContestsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-approved-contests',
                                    name: 'WorkSpaceMyApprovedContestsPage',
                                    component: MyApprovedContestsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-contests',
                                    name: 'WorkSpaceMyContestsPage',
                                    component: MyContestsComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'edit-contest/:contest_id?',
                                    name: 'WorkSpaceEditContestPage',
                                    component: ContestEditComponent,
                                    props: true,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                            ]
                        },
                        {
                            path: 'tasks',
                            name: 'WorkSpaceTasksPage',
                            component: TasksMainComponent,
                            meta: {
                                authorize: ['user']
                            },
                            children: [
                                {
                                    path: 'my-pending-tasks',
                                    name: 'WorkSpaceMyPendingTasksPage',
                                    component: MyPendingTasksComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-rejected-tasks',
                                    name: 'WorkSpaceMyRejectedTasksPage',
                                    component: MyRejectedTasksComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-approved-tasks',
                                    name: 'WorkSpaceMyApprovedTasksPage',
                                    component: MyApprovedTasksComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'all-tasks',
                                    name: 'WorkSpaceAllTasksPage',
                                    component: MyTasksComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'edit-task/:task_id?',
                                    name: 'WorkSpaceEditTaskPage',
                                    component: TaskEditComponent,
                                    props: true,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                            ]
                        },
                        {
                            path: 'checkers',
                            name: 'WorkSpaceCheckersPage',
                            component: CheckersMainComponent,
                            meta: {
                                authorize: ['user']
                            },
                            children: [
                                {
                                    path: 'my-pending-checkers',
                                    name: 'WorkSpaceMyPendingCheckersPage',
                                    component: MyPendingCheckersComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-rejected-checkers',
                                    name: 'WorkSpaceMyRejectedCheckersPage',
                                    component: MyRejectedCheckersComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'my-approved-checkers',
                                    name: 'WorkSpaceMyApprovedCheckersPage',
                                    component: MyApprovedCheckersComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'all-checkers',
                                    name: 'WorkSpaceAllCheckersPage',
                                    component: MyCheckersComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'edit-checker/:id?',
                                    name: 'WorkSpaceEditCheckersPage',
                                    component: CheckerEditComponent,
                                    props: true,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                            ]
                        },
                        {
                            path: 'rule-sets',
                            name: 'WorkSpaceRuleSetsPage',
                            component: MainRuleSetsComponent,
                            meta: {
                                authorize: ['user']
                            },
                            children: [
                                {
                                    path: 'my-rule-sets',
                                    name: 'WorkSpaceMyRuleSetsPage',
                                    component: RuleSetsListComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'all-rule-sets',
                                    name: 'WorkSpaceAllRuleSetsPage',
                                    component: AllRuleSetsListComponent,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                                {
                                    path: 'edit-rule-set/:set_id?',
                                    name: 'WorkSpaceEditRuleSetPage',
                                    component: RuleSetEditComponent,
                                    props: true,
                                    meta: {
                                        authorize: ['user']
                                    },
                                },
                            ]
                        },
                    ]
                },
            ],
        },
        {
            path: '/roles',
            name: 'RoleSelector',
            component: RolesPage,
            meta: {
                authorize: true
            }
        },
        {
            path: '/login',
            name: 'Login',
            component: LoginPage,
            props: true,
        },
        {
            path: '/register',
            name: 'Register',
            component: RegisterPage,
            props: true,
        },
    ],
})
router.beforeEach(async (to, from, next) => {
    await store.dispatch('initAuth') // ensure auth synced with server
    let isAuthenticated = store.getters.isAuthenticated
    if (isAuthenticated) {
        if (['Login', 'Register'].includes(to.name)) {
            return next({name: 'Home'})
        }
    }
    const {authorize} = to.meta;
    if (authorize) {
        if (!isAuthenticated) {
            return next({name: 'UserStarterPage', query: {returnUrl: to.path}})
        }

        if (authorize.length && !authorize.includes(store.getters.currentRole)) {
            // нет нужной роли
            await store.dispatch('globalAlert', {message: 'Вы куда это?)'})
            return next(false)
        }
    }
    next()
})
export default router
