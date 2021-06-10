import WorkSpacePage from "../../../views/user/workspace/WorkSpacePage";
import WorkSpaceWelcomeComponent from "../../../components/user/workspace/WorkSpaceWelcomeComponent";
import AboutComponent from "../../../components/user/workspace/AboutComponent";
import PostsMainComponent from "../../../components/user/workspace/posts/PostsMainComponent";
import MyPendingPostsComponent from "../../../components/user/workspace/posts/MyPendingPostsComponent";
import MyRejectedPostsComponent from "../../../components/user/workspace/posts/MyRejectedPostsComponent";
import MyApprovedPostsComponent from "../../../components/user/workspace/posts/MyApprovedPostsComponent";
import MyPostsComponent from "../../../components/user/workspace/posts/MyPostsComponent";
import ContestsMainComponent from "../../../components/user/workspace/contests/ContestsMainComponent";
import MyPendingContestsComponent from "../../../components/user/workspace/contests/MyPendingContestsComponent";
import MyRejectedContestsComponent from "../../../components/user/workspace/contests/MyRejectedContestsComponent";
import MyApprovedContestsComponent from "../../../components/user/workspace/contests/MyApprovedContestsComponent";
import MyContestsComponent from "../../../components/user/workspace/contests/MyContestsComponent";
import ContestEditComponent from "../../../components/user/workspace/contests/ContestEditComponent";
import TasksMainComponent from "../../../components/user/workspace/tasks/TasksMainComponent";
import MyPendingTasksComponent from "../../../components/user/workspace/tasks/MyPendingTasksComponent";
import MyRejectedTasksComponent from "../../../components/user/workspace/tasks/MyRejectedTasksComponent";
import MyApprovedTasksComponent from "../../../components/user/workspace/tasks/MyApprovedTasksComponent";
import MyTasksComponent from "../../../components/user/workspace/tasks/MyTasksComponent";
import TaskEditComponent from "../../../components/user/workspace/tasks/TaskEditComponent";
import CheckersMainComponent from "../../../components/user/workspace/checkers/CheckersMainComponent";
import MyPendingCheckersComponent from "../../../components/user/workspace/checkers/MyPendingCheckersComponent";
import MyRejectedCheckersComponent from "../../../components/user/workspace/checkers/MyRejectedCheckersComponent";
import MyApprovedCheckersComponent from "../../../components/user/workspace/checkers/MyApprovedCheckersComponent";
import MyCheckersComponent from "../../../components/user/workspace/checkers/MyCheckersComponent";
import CheckerEditComponent from "../../../components/user/workspace/checkers/CheckerEditComponent";
import MainRuleSetsComponent from "../../../components/user/workspace/rule_sets/MainRuleSetsComponent";
import RuleSetsListComponent from "../../../components/user/workspace/rule_sets/RuleSetsListComponent";
import AllRuleSetsListComponent from "../../../components/user/workspace/rule_sets/AllRuleSetsListComponent";
import RuleSetEditComponent from "../../../components/user/workspace/rule_sets/RuleSetEditComponent";

export default {
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
}