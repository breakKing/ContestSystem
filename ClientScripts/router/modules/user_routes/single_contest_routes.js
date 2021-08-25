import ContestPage from "../../../views/user/contests/single_contest/ContestPage";
import ContestInformationComponent from "../../../components/user/contests/ContestInformationComponent";
import TaskComponent from "../../../components/user/contests/participating/TaskComponent";
import ContestMySolutionsListComponent
    from "../../../components/user/contests/participating/ContestMySolutionsListComponent";
import ContestMonitoringComponent from "../../../components/user/contests/participating/ContestMonitoringComponent";
import SolutionViewComponent from "../../../components/user/contests/participating/SolutionViewComponent";
import ContestChatComponent from "../../../components/user/contests/participating/ContestChatComponent";

export default {
    path: 'contest',
    name: 'MainContestPage',
    component: ContestPage,
    meta: {
        authorize: ['user']
    },
    children: [
        {
            path: ':contest_id',
            name: 'ContestPage',
            component: ContestInformationComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: ':contest_id/participate/chatting/:chat_id',
            name: 'ContestParticipatingChattingPage',
            component: ContestChatComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: ':contest_id/participate/:task_id?',
            name: 'ContestParticipatingPage',
            component: TaskComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: ':contest_id/monitor',
            name: 'ContestMonitorPage',
            component: ContestMonitoringComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: ':contest_id/my-solutions',
            name: 'ContestMySolutionsPage',
            component: ContestMySolutionsListComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: ':contest_id/my-solutions/:solution_id',
            name: 'SolutionViewPage',
            component: SolutionViewComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: 'organizer/:contest_id/solutions/:solution_id',
            name: 'OrganizerSolutionViewPage',
            component: SolutionViewComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
    ]
}
